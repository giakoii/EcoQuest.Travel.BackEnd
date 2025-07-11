using BackEnd.DTOs.Ecq110;
using BackEnd.DTOs.Ecq200;
using BackEnd.DTOs.Ecq210;
using BackEnd.DTOs.Ecq220;
using BackEnd.DTOs.Ecq230;
using BackEnd.Logics;
using BackEnd.Models;
using BackEnd.SystemClient;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BackEnd.Services;

public class TripScheduleService : ITripScheduleService
{
    private readonly TripScheduleRepositoryWrapper _repositoryWrapper;
    private readonly AiLogic _aiLogic;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="repositoryWrapper"></param>
    /// <param name="aiLogic">AI logic service</param>
    public TripScheduleService(TripScheduleRepositoryWrapper repositoryWrapper, AiLogic aiLogic)
    {
        _repositoryWrapper = repositoryWrapper;
        _aiLogic = aiLogic;
    }
    /// <summary>
    /// Insert a trip schedule
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq110InsertTripScheduleResponse> InsertTripSchedule(Ecq110InsertTripScheduleRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq110InsertTripScheduleResponse { Success = false };

        // Validate trip existence and ownership
        var trip = await ValidateTripAsync(request.TripId, identityEntity, response);
        if (trip == null) return response;

        // Get service type mapping
        var serviceTypeDict = await GetServiceTypeMappingAsync(request);

        // Check hotel service conditions (check-in/out, room availability)
        if (serviceTypeDict != null)
        {
            var hotelServiceIds = serviceTypeDict
                .Where(kv => kv.Value == ConstantEnum.EntityType.Hotel.ToString())
                .Select(kv => kv.Key);

            foreach (var hotelId in hotelServiceIds)
            {
                var hotelDates = request.TripScheduleDetails
                    .Where(s => s.ServiceId == hotelId)
                    .Select(s => s.ScheduleDate)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                if (hotelDates.Count < 2)
                {
                    response.SetMessage(MessageId.I00000, "You must schedule both check-in and check-out dates for the hotel.");
                    return response;
                }

                var estimatedCostGuest = trip.TotalEstimatedCost;
                var hotelRoomAvailable = await _repositoryWrapper.HotelRoomRepository
                    .Find(hr => hr.HotelId == hotelId && hr.IsActive == true && hr.IsAvailable == true)
                    .ToListAsync();

                if (!hotelRoomAvailable.Any())
                {
                    response.SetMessage(MessageId.I00000, "No available hotel rooms for the specified dates.");
                    return response;
                }

                int stayNights = hotelDates.Last().DayNumber - hotelDates.First().DayNumber;
                if (!IsValidRoomCombination(hotelRoomAvailable, trip.NumberOfPeople ?? 0, estimatedCostGuest ?? decimal.MaxValue, stayNights))
                {
                    response.SetMessage(MessageId.I00000, "No combination of hotel rooms available for the specified dates and number of guests within the estimated cost.");
                    return response;
                }
            }
        }

        // Prepare to cache estimated cost per service per date
        var serviceCostMap = new Dictionary<(Guid, DateOnly), decimal>();

        // Validate total trip cost against budget, and fill serviceCostMap
        if (!await ValidateTotalCostAsync(request, trip, serviceTypeDict!, response, serviceCostMap))
        {
            return response;
        }

        // Insert trip schedule into DB
        await _repositoryWrapper.TripScheduleRepository.ExecuteInTransactionAsync(async () =>
        {
            foreach (var detail in request.TripScheduleDetails)
            {
                if (detail.ScheduleDate < trip.StartDate || detail.ScheduleDate > trip.EndDate)
                {
                    response.SetMessage(MessageId.I00000, "Schedule date cannot be before trip start date or after trip end date.");
                    return false;
                }

                var serviceId = detail.ServiceId;
                string? serviceType = null;
                decimal? estimatedCost = detail.EstimatedCost;

                if (serviceId.HasValue)
                {
                    serviceTypeDict!.TryGetValue(serviceId.Value, out serviceType);
                    serviceCostMap.TryGetValue((serviceId.Value, detail.ScheduleDate), out var cost);
                    estimatedCost = cost;
                }

                var latLong = await GetLatLongUsingNominatimAsync(detail.Address);

                var tripSchedule = new TripSchedule
                {
                    TripId = request.TripId,
                    ScheduleDate = detail.ScheduleDate,
                    Title = detail.Title,
                    Description = detail.Description,
                    StartTime = detail.StartTime,
                    EndTime = detail.EndTime,
                    Location = latLong != null ? $"{latLong.Value.lat},{latLong.Value.lng}" : null,
                    ServiceId = serviceId,
                    ServiceType = serviceType,
                    EstimatedCost = estimatedCost
                };

                await _repositoryWrapper.TripScheduleRepository.AddAsync(tripSchedule);
            }

            await _repositoryWrapper.TripScheduleRepository.SaveChangesAsync(identityEntity.Email);
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });

        return response;
    }

    #region Sub Functions of InsertTripSchedule

    /// <summary>
    /// Validate trip existence and ownership
    /// </summary>
    /// <param name="tripId"></param>
    /// <param name="identityEntity"></param>
    /// <param name="response"></param>
    /// <returns></returns>
    private async Task<Trip?> ValidateTripAsync(Guid tripId, IdentityEntity identityEntity, Ecq110InsertTripScheduleResponse response)
    {
        var trip = await _repositoryWrapper.TripRepository.Find(x => x.TripId == tripId && x.IsActive == true).FirstOrDefaultAsync();
        if (trip == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.TripNotFound);
            return null;
        }

        if (trip.UserId != Guid.Parse(identityEntity.UserId))
        {
            response.SetMessage(MessageId.I00000, CommonMessages.NotAuthorizedToManageTrip);
            return null;
        }

        return trip;
    }
    
    /// <summary>
    /// Get service type mapping for trip schedule details
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task<Dictionary<Guid, string>?> GetServiceTypeMappingAsync(Ecq110InsertTripScheduleRequest request)
    {
        var serviceTypeDict = new Dictionary<Guid, string>();
        var serviceIds = request.TripScheduleDetails.Where(s => s.ServiceId != null).Select(s => s.ServiceId!.Value)
            .Distinct();
        foreach (var id in serviceIds)
        {
            var hotel = await _repositoryWrapper.HotelRepository.Find(x => x.HotelId == id && x.IsActive == true).FirstOrDefaultAsync();
            if (hotel != null)
            {
                serviceTypeDict[id] = ConstantEnum.EntityType.Hotel.ToString();
                continue;
            }

            var restaurant = await _repositoryWrapper.RestaurantDetailRepository.Find(x => x.RestaurantId == id && x.IsActive == true)
                .FirstOrDefaultAsync();
            if (restaurant != null)
            {
                serviceTypeDict[id] = ConstantEnum.EntityType.Restaurant.ToString();
                continue;
            }

            var attraction = await _repositoryWrapper.AttractionDetailRepository.Find(x => x.AttractionId == id && x.IsActive == true)
                .FirstOrDefaultAsync();
            if (attraction != null)
            {
                serviceTypeDict[id] = ConstantEnum.EntityType.Attraction.ToString();
            }
        }

        return serviceTypeDict;
    }

    /// <summary>
    /// Validate total cost of trip schedule against estimated budget
    /// </summary>
    /// <param name="request"></param>
    /// <param name="trip"></param>
    /// <param name="serviceTypeDict"></param>
    /// <param name="response"></param>
    /// <param name="serviceCostMap"></param>
    /// <returns></returns>
    private async Task<bool> ValidateTotalCostAsync(
        Ecq110InsertTripScheduleRequest request,
        Trip trip,
        Dictionary<Guid, string> serviceTypeDict,
        Ecq110InsertTripScheduleResponse response,
        Dictionary<(Guid, DateOnly), decimal> serviceCostMap)
    {
        if (serviceCostMap == null) throw new ArgumentNullException(nameof(serviceCostMap));
        decimal totalCost = 0;

        // Sum all custom estimated costs from user input
        var customCosts = request.TripScheduleDetails
            .Where(x => x.ServiceId == null && x.EstimatedCost.HasValue)
            .Sum(x => x.EstimatedCost!.Value);
        totalCost += customCosts;

        // Group schedule details by service ID
        var groupedByService = request.TripScheduleDetails
            .Where(x => x.ServiceId != null)
            .GroupBy(x => x.ServiceId!.Value);

        foreach (var group in groupedByService)
        {
            var serviceId = group.Key;
            var type = serviceTypeDict[serviceId];
            var days = group.Select(g => g.ScheduleDate).Distinct().ToList();
            decimal cost = 0;

            // Type is Hotel
            if (type == ConstantEnum.EntityType.Hotel.ToString())
            {
                // Validate room availability and calculate cost
                var rooms = await _repositoryWrapper.HotelRoomRepository.Find(hr =>
                        hr.HotelId == serviceId && hr.IsAvailable == true && hr.IsActive == true)
                    .OrderBy(r => r!.PricePerNight).ToListAsync();

                int guestRemaining = trip.NumberOfPeople ?? 0;
                var selectedRooms = new List<HotelRoom>();

                foreach (var room in rooms)
                {
                    selectedRooms.Add(room!);
                    guestRemaining -= room!.MaxGuests;
                    if (guestRemaining <= 0) break;
                }

                if (guestRemaining > 0)
                {
                    response.SetMessage(MessageId.I00000, $"Not enough hotel rooms for {trip.NumberOfPeople} guests.");
                    return false;
                }

                var nights = days.Last().DayNumber - days.First().DayNumber;
                if (nights < 1) nights = 1;

                foreach (var room in selectedRooms)
                {
                    cost += nights * room.PricePerNight;
                }

                var dailyCost = cost / nights;
                foreach (var day in days)
                {
                    serviceCostMap[(serviceId, day)] = dailyCost;
                }
            }
            
            // Type is Restaurant
            else if (type == ConstantEnum.EntityType.Restaurant.ToString())
            {
                var restaurant = await _repositoryWrapper.RestaurantDetailRepository
                    .Find(r => r.RestaurantId == serviceId && r.IsActive == true).FirstOrDefaultAsync();

                if (restaurant != null && restaurant.MinPrice.HasValue)
                {
                    cost = restaurant.MinPrice.Value * (trip.NumberOfPeople ?? 0);
                    foreach (var day in days)
                    {
                        serviceCostMap[(serviceId, day)] = cost / days.Count;
                    }
                }
            }
            
            // Type is Attraction
            else if (type == ConstantEnum.EntityType.Attraction.ToString())
            {
                var attraction = await _repositoryWrapper.AttractionDetailRepository
                    .Find(a => a.AttractionId == serviceId && a.IsActive == true).FirstOrDefaultAsync();

                if (attraction != null && attraction.TicketPrice.HasValue)
                {
                    cost = attraction.TicketPrice.Value * (trip.NumberOfPeople ?? 0);
                    foreach (var day in days)
                    {
                        serviceCostMap[(serviceId, day)] = cost / days.Count;
                    }
                }
            }

            totalCost += cost;
        }

        if (trip.TotalEstimatedCost.HasValue && totalCost > trip.TotalEstimatedCost.Value)
        {
            response.SetMessage(MessageId.I00000, $"Your planned schedule cost ({totalCost:C}) exceeds your estimated budget ({trip.TotalEstimatedCost:C}).");
            return false;
        }

        return true;
    }


    /// <summary>
    /// Check if there is a valid combination of hotel rooms
    /// that can accommodate the required number of guests within the budget
    /// </summary>
    /// <param name="availableRooms"></param>
    /// <param name="requiredGuests"></param>
    /// <param name="maxBudget"></param>
    /// <param name="stayNights"></param>
    /// <returns></returns>
    private bool IsValidRoomCombination(List<HotelRoom?> availableRooms, int requiredGuests, decimal maxBudget, int stayNights)
    {
        var allCombinations = GetAllRoomCombinations(availableRooms!);

        foreach (var combination in allCombinations)
        {
            int totalCapacity = combination.Sum(r => r.MaxGuests);
            decimal totalCost = combination.Sum(r => r.PricePerNight * stayNights);

            if (totalCapacity >= requiredGuests && totalCost <= maxBudget)
            {
                return true;
            }
        }

        return false;
    }

    List<List<HotelRoom>> GetAllRoomCombinations(List<HotelRoom> rooms)
    {
        var results = new List<List<HotelRoom>>();
        int count = rooms.Count;
        for (int i = 1; i < (1 << count); i++)
        {
            var combo = new List<HotelRoom>();
            for (int j = 0; j < count; j++)
            {
                if ((i & (1 << j)) != 0)
                    combo.Add(rooms[j]);
            }

            results.Add(combo);
        }

        return results;
    }

    /// <summary>
    /// Convert address to latitude and longitude using Nominatim
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    private async Task<(double lat, double lng)?> GetLatLongUsingNominatimAsync(string address)
    {
        var encodedAddress = Uri.EscapeDataString(address);
        var url = $"https://nominatim.openstreetmap.org/search?q={encodedAddress}&format=json&limit=1";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp/1.0");

        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        var results = JsonConvert.DeserializeObject<List<dynamic>>(content);

        if (results?.Count > 0)
        {
            double lat = double.Parse((string)results[0].lat);
            double lon = double.Parse((string)results[0].lon);
            return (lat, lon);
        }

        return null;
    }

    #endregion

    public async Task<Ecq110InsertTripScheduleWithAiResponse> InsertTripScheduleUseAi(Ecq110InsertTripScheduleWithAiRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq110InsertTripScheduleWithAiResponse { Success = false };

        try
        {
            // Check if trip exists
            var trip = await _repositoryWrapper.TripRepository.Find(x => x.TripId == request.TripId && x.IsActive == true, false, x => x.TripDestinations)
                .FirstOrDefaultAsync();
            if (trip == null)
            {
                response.SetMessage(MessageId.I00000, CommonMessages.TripNotFound);
                return response;
            }

            // Verify ownership
            if (trip.UserId != Guid.Parse(identityEntity.UserId))
            {
                response.SetMessage(MessageId.I00000, CommonMessages.NotAuthorizedToManageTrip);
                return response;
            }

            // Get destination IDs for filtering
            var destinationIds = trip.TripDestinations.Select(td => td.DestinationId).ToList();
            
            if (!destinationIds.Any())
            {
                response.SetMessage(MessageId.I00000, "No destinations found for this trip");
                return response;
            }

            // Get data efficiently - filter at database level
            var hotels = await _repositoryWrapper.HotelRepository.GetView<VwHotel>()
                .Where(x => destinationIds.Contains(x.DestinationId!.Value))
                .ToListAsync();
                
            var restaurants = await _repositoryWrapper.RestaurantDetailRepository.GetView<VwRestaurant>()
                .Where(x => destinationIds.Contains(x.DestinationId!.Value))
                .ToListAsync();
                
            var attractions = await _repositoryWrapper.AttractionDetailRepository.GetView<VwAttraction>()
                .Where(x => destinationIds.Contains(x.DestinationId!.Value))
                .ToListAsync();

            // Get destinations
            var tripDestinations = await _repositoryWrapper.TripDestinationRepository
                .Find(x => x.TripId == trip.TripId, false, x => x.Destination)
                .ToListAsync();

            // Convert to AI entities
            var hotelList = hotels.Select(h => new Ecq210HotelEntity
            {
                HotelId = h.HotelId,
                Name = h.HotelName,
                Description = h.HotelDescription,
                AddressLine = h.AddressLine,
                Ward = h.Ward,
                District = h.District,
                Province = h.Province,
                DestinationId = h.DestinationId,
                DestinationName = h.DestinationName,
                AverageRating = h.AverageRating,
                TotalRatings = h.TotalRatings,
                MinPrice = h.MinPrice,
                MaxPrice = h.MaxPrice,
            }).ToList();

            var restaurantList = restaurants.Select(r => new Ecq220RestaurantEntity
            {
                RestaurantId = r.RestaurantId,
                RestaurantName = r.RestaurantName!,
                AddressLine = r.Address,
                OpenTime = StringUtil.ConvertToHhMm(r.OpenTime),
                CloseTime = StringUtil.ConvertToHhMm(r.CloseTime),
                MinPrice = r.MinPrice,
                MaxPrice = r.MaxPrice,
                DestinationId = r.DestinationId,
                DestinationName = r.DestinationName,
                AverageRating = r.AverageRating,
                TotalRatings = r.TotalRatings
            }).ToList();

            var attractionList = attractions.Select(a => new Ecq230AttractionDetailEntity
            {
                AttractionId = a.AttractionId,
                AttractionName = a.AttractionName,
                AttractionType = a.AttractionType,
                TicketPrice = a.TicketPrice,
                DestinationId = a.DestinationId,
                DestinationName = a.DestinationName,
                AverageRating = a.AverageRating,
                TotalRatings = a.TotalRatings,
            }).ToList();

            var destinationList = tripDestinations.Select(destination => new Ecq200DestinationDetailEntity
            {
                DestinationId = destination!.DestinationId,
                Name = destination.Destination.Name,
                Description = destination.Destination.Description,
                AddressLine = destination.Destination.AddressLine,
                District = destination.Destination.District,
                Province = destination.Destination.Province
            }).ToList();

            // Convert dates to strings for AI
            var startDateStr = trip.StartDate?.ToString("yyyy-MM-dd") ?? string.Empty;
            var endDateStr = trip.EndDate?.ToString("yyyy-MM-dd") ?? string.Empty;
            var numberOfPeopleStr = trip.NumberOfPeople?.ToString() ?? "1";
            var totalEstimatedCostStr = trip.TotalEstimatedCost?.ToString("F0") ?? "0";

            // Call AI to generate schedule
            var generateSchedule = await _aiLogic.AskAiModel(
                hotelList, 
                attractionList, 
                restaurantList, 
                destinationList, 
                startDateStr,
                endDateStr, 
                numberOfPeopleStr,
                totalEstimatedCostStr);
            
            response.Response = generateSchedule;
            response.Success = true;
            response.SetMessage(MessageId.I00001);
        }
        catch (Exception ex)
        {
            response.SetMessage(MessageId.I00000, $"Error generating AI schedule: {ex.Message}");
            response.Success = false;
        }

        return response;
    }


    /// <summary>
    /// Select a specific trip schedule by tripId
    /// </summary>
    /// <param name="tripId">Schedule ID</param>
    /// <returns>Trip schedule details</returns>
    public async Task<Ecq110SelectTripScheduleResponse> SelectTripSchedule(Guid tripId)
    {
        var response = new Ecq110SelectTripScheduleResponse { Success = false };

        // Find trip schedule by ID
        var schedule = await _repositoryWrapper.TripScheduleRepository.GetView<VwTripSchedule>(x => x.TripId == tripId)
            .Select(x => new Ecq110TripScheduleEntity
            {
                ScheduleId = x.ScheduleId,
                TripId = x.TripId,
                ScheduleDate = StringUtil.ConvertToDateAsDdMmYyyy(x.ScheduleDate),
                Title = x.ScheduleTitle,
                Description = x.ScheduleDescription!,
                StartTime = StringUtil.ConvertToHhMm(x.StartTime),
                EndTime = StringUtil.ConvertToHhMm(x.EndTime),
                Location = x.Location!,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.UpdatedAt),
                ServiceId = x.ServiceId,
                ServiceType = x.ServiceType!,
                EstimatedCost = x.EstimatedCost,
            })
            .ToListAsync();

        schedule = schedule
            .OrderBy(x => x.ScheduleDate)
            .ThenBy(x => x.StartTime)
            .ToList();

        // True
        response.Response = schedule!;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }

    /// <summary>
    /// Select all schedules of user
    /// </summary>
    /// <param name="userId">Trip ID</param>
    /// <returns>List of trip schedules</returns>
    public async Task<Ecq110SelectTripSchedulesResponse> SelectTripSchedules(Guid userId)
    {
        var response = new Ecq110SelectTripSchedulesResponse { Success = false };

        var schedules = await _repositoryWrapper.TripScheduleRepository.GetView<VwTripSchedule>(x => x.UserId == userId).ToListAsync();

        var grouped = schedules
            .GroupBy(x => new { x.TripId, x.TripName })
            .Select(group => new Ecq110TripSchedulesEntity
            {
                TripId = group.Key.TripId,
                TripName = group.Key.TripName!,
                TripScheduleDetails = group.Select(x => new Ecq110TripScheduleDetail
                    {
                        ScheduleDate = StringUtil.ConvertToDateAsDdMmYyyy(x.ScheduleDate),
                        Title = x.ScheduleTitle,
                        Description = x.ScheduleDescription!,
                        StartTime = StringUtil.ConvertToHhMm(x.StartTime),
                        EndTime = StringUtil.ConvertToHhMm(x.EndTime),
                        Location = x.Location!,
                        EstimatedCost = x.EstimatedCost,
                        ServiceId = x.ServiceId,
                        ServiceType = x.ServiceType!,
                        CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.CreatedAt),
                        UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.UpdatedAt)
                    })
                    .OrderBy(x => x.ScheduleDate)
                    .ThenBy(x => x.StartTime)
                    .ToList()
            }).ToList();

        // Set response
        response.Response = grouped;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }

    /// <summary>
    /// Update an existing trip schedule
    /// </summary>
    /// <param name="request">Update request</param>
    /// <param name="identityEntity">User identity</param>
    /// <returns>Update result</returns>
    public async Task<Ecq110UpdateTripScheduleResponse> UpdateTripSchedule(Ecq110UpdateTripScheduleRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq110UpdateTripScheduleResponse { Success = false };

        // Find trip by ID
        var trip = await _repositoryWrapper.TripRepository.Find(x => x.TripId == request.TripId && x.IsActive == true).FirstOrDefaultAsync();
        if (trip == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.TripNotFound);
            return response;
        }

        // Verify ownership
        if (trip.UserId != Guid.Parse(identityEntity.UserId))
        {
            response.SetMessage(MessageId.I00000, CommonMessages.NotAuthorizedToManageTrip);
            return response;
        }

        // // Check if schedule date is within trip date range
        // if (request.ScheduleDate < trip.StartDate || request.ScheduleDate > trip.EndDate)
        // {
        //     response.SetMessage(MessageId.I00000, "Schedule date must be within trip date range");
        //     return response;
        // }

        // Find schedule by ID
        var schedule = await _repositoryWrapper.TripScheduleRepository.Find(x => x.ScheduleId == request.ScheduleId && x.IsActive).FirstOrDefaultAsync();
        if (schedule == null)
        {
            response.SetMessage(MessageId.I00000, "Trip schedule not found");
            return response;
        }

        // Check if schedule belongs to the trip
        if (schedule.TripId != request.TripId)
        {
            response.SetMessage(MessageId.I00000, "Schedule does not belong to the specified trip");
            return response;
        }

        // // Update schedule properties
        // schedule.ScheduleDate = request.ScheduleDate;
        // schedule.Title = request.Title;
        // schedule.Description = request.Description;
        // schedule.StartTime = request.StartTime;
        // schedule.EndTime = request.EndTime;
        // schedule.Location = request.Location;
        //
        // Save changes
        await _repositoryWrapper.TripScheduleRepository.UpdateAsync(schedule);
        await _repositoryWrapper.TripScheduleRepository.SaveChangesAsync(identityEntity.Email);

        // Set response
        response.Response = true;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }

    /// <summary>
    /// Delete a trip schedule (soft delete)
    /// </summary>
    /// <param name="request">Delete request</param>
    /// <param name="identityEntity">User identity</param>
    /// <returns>Delete result</returns>
    public async Task<Ecq110DeleteTripScheduleResponse> DeleteTripSchedule(Ecq110DeleteTripScheduleRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq110DeleteTripScheduleResponse { Success = false };

        // Find schedule by ID
        var schedule = await _repositoryWrapper.TripScheduleRepository.Find(x => x.ScheduleId == request.ScheduleId && x.IsActive).FirstOrDefaultAsync();
        if (schedule == null)
        {
            response.SetMessage(MessageId.I00000, "Trip schedule not found");
            return response;
        }

        // Find trip to verify ownership
        var trip = await _repositoryWrapper.TripRepository.Find(x => x.TripId == schedule.TripId && x.IsActive == true).FirstOrDefaultAsync();
        if (trip == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.TripNotFound);
            return response;
        }

        // Verify ownership
        if (trip.UserId != Guid.Parse(identityEntity.UserId))
        {
            response.SetMessage(MessageId.I00000, CommonMessages.NotAuthorizedToManageTrip);
            return response;
        }

        // Soft delete the schedule
        await _repositoryWrapper.TripScheduleRepository.UpdateAsync(schedule);
        await _repositoryWrapper.TripScheduleRepository.SaveChangesAsync(identityEntity.Email, true);

        // Set response
        response.Response = true;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }

    /// <summary>
    /// Select services based on service type for Ecq110
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Ecq110SelectServiceResponse> SelectService(Ecq110SelectServiceRequest request)
    {
        var response = new Ecq110SelectServiceResponse { Success = false };
        
        List<Ecq110SelectServiceEntity> entityResponse;
        
        // Validate request
        if (request.ServiceType == ((int) ConstantEnum.ServiceType.Hotel))
        {
            // Get hotels
            var hotels = await _repositoryWrapper.HotelRepository.GetView<VwHotel>()
                .Select(x => new Ecq110SelectServiceEntity
                {
                    ServiceId = x.HotelId,
                    ServiceName = x.HotelName,
                    Address = x.Address!,
                    Cost = $"{x.MinPrice} - {x.MaxPrice}",
                }).ToListAsync();
            entityResponse = hotels;
        }
        else if (request.ServiceType == ((int) ConstantEnum.ServiceType.Restaurant))
        {
            // Get restaurants
            var restaurants = await _repositoryWrapper.RestaurantDetailRepository.GetView<VwRestaurant>()
                .Select(x => new Ecq110SelectServiceEntity
                {
                    ServiceId = x.RestaurantId,
                    ServiceName = x.RestaurantName!,
                    Address = x.Address!,
                    Cost = $"{x.MinPrice} - {x.MaxPrice}",
                }).ToListAsync();
            entityResponse = restaurants;
        }
        else if (request.ServiceType == ((int) ConstantEnum.ServiceType.Attraction))
        {
            // Get attractions
            var attractions = await _repositoryWrapper.AttractionDetailRepository.GetView<VwAttraction>()
                .Select(x => new Ecq110SelectServiceEntity
                {
                    ServiceId = x.AttractionId,
                    ServiceName = x.AttractionName!,
                    Address = x.Address!,
                    Cost = $"{x.TicketPrice}",
                }).ToListAsync();
            entityResponse = attractions;
        }
        else
        {
            response.SetMessage(MessageId.I00000, "Invalid service type");
            return response;
            
        }
        
        // True
        response.Success = true;
        response.Response = entityResponse;
        response.SetMessage(MessageId.I00001);
        return response;
    }
}
