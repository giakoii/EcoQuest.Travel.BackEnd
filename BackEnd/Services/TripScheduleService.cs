using BackEnd.DTOs.Chatbot;
using BackEnd.DTOs.Ecq110;
using BackEnd.DTOs.Ecq210;
using BackEnd.DTOs.Ecq220;
using BackEnd.DTOs.Ecq230;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.SystemClient;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BackEnd.Services;

public class TripScheduleService : ITripScheduleService
{
    private readonly IBaseRepository<Trip, Guid> _tripRepository;
    private readonly IBaseRepository<TripSchedule, Guid> _tripScheduleRepository;
    private readonly IBaseRepository<AttractionDetail, Guid> _attractionDetailRepository;
    private readonly IBaseRepository<RestaurantDetail, Guid> _restaurantDetailRepository;
    private readonly IBaseRepository<Hotel, Guid> _hotelRepository;
    private readonly IBaseRepository<HotelRoom, Guid> _hotelRoomRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tripRepository"></param>
    /// <param name="tripScheduleRepository"></param>
    /// <param name="attractionDetailRepository"></param>
    /// <param name="restaurantDetailRepository"></param>
    /// <param name="hotelRepository"></param>
    /// <param name="hotelRoomRepository"></param>
    public TripScheduleService(IBaseRepository<Trip, Guid> tripRepository,
        IBaseRepository<TripSchedule, Guid> tripScheduleRepository,
        IBaseRepository<AttractionDetail, Guid> attractionDetailRepository,
        IBaseRepository<RestaurantDetail, Guid> restaurantDetailRepository,
        IBaseRepository<Hotel, Guid> hotelRepository, IBaseRepository<HotelRoom, Guid> hotelRoomRepository)
    {
        _tripRepository = tripRepository;
        _tripScheduleRepository = tripScheduleRepository;
        _attractionDetailRepository = attractionDetailRepository;
        _restaurantDetailRepository = restaurantDetailRepository;
        _hotelRepository = hotelRepository;
        _hotelRoomRepository = hotelRoomRepository;
    }

    /// <summary>
    /// Insert a trip schedule
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq110InsertTripScheduleResponse> InsertTripSchedule(Ecq110InsertTripScheduleRequest request,
        IdentityEntity identityEntity)
    {
        var response = new Ecq110InsertTripScheduleResponse { Success = false };

        // Validate trip existence and ownership
        var trip = await ValidateTripAsync(request.TripId, identityEntity, response);
        if (trip == null) return response;

        // Validate that all dates within the trip's start and end dates are scheduled
        if (!ValidateFullScheduleDates(request, trip, response)) return response;

        // Get service type mapping
        var serviceTypeDict = await GetServiceTypeMappingAsync(request, response);

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
                    response.SetMessage(MessageId.I00000,
                        "You must schedule both check-in and check-out dates for the hotel.");
                    return response;
                }

                var estimatedCostGuest = trip.TotalEstimatedCost;
                var hotelRoomAvailable = await _hotelRoomRepository
                    .Find(hr => hr.HotelId == hotelId && hr.IsActive == true && hr.IsAvailable == true)
                    .ToListAsync();

                if (!hotelRoomAvailable.Any())
                {
                    response.SetMessage(MessageId.I00000, "No available hotel rooms for the specified dates.");
                    return response;
                }

                int stayNights = hotelDates.Last().DayNumber - hotelDates.First().DayNumber;
                if (!IsValidRoomCombination(hotelRoomAvailable, trip.NumberOfPeople ?? 0,
                        estimatedCostGuest ?? decimal.MaxValue, stayNights, out _))
                {
                    response.SetMessage(MessageId.I00000,
                        "No combination of hotel rooms available for the specified dates and number of guests within the estimated cost.");
                    return response;
                }
            }
        }

        // Prepare to cache estimated cost per service per date
        var serviceCostMap = new Dictionary<(Guid, DateOnly), decimal>();

        // Validate total trip cost against budget, and fill serviceCostMap
        if (!await ValidateTotalCostAsync(request, trip, serviceTypeDict, response, serviceCostMap))
        {
            return response;
        }

        // Insert trip schedule into DB
        await _tripScheduleRepository.ExecuteInTransactionAsync(async () =>
        {
            foreach (var detail in request.TripScheduleDetails)
            {
                if (detail.ScheduleDate < trip.StartDate || detail.ScheduleDate > trip.EndDate)
                {
                    response.SetMessage(MessageId.I00000,
                        "Schedule date cannot be before trip start date or after trip end date.");
                    return false;
                }

                var serviceId = detail.ServiceId;
                string? serviceType = null;
                decimal? estimatedCost = detail.EstimatedCost;

                if (serviceId.HasValue)
                {
                    serviceTypeDict.TryGetValue(serviceId.Value, out serviceType);
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

                await _tripScheduleRepository.AddAsync(tripSchedule);
            }

            await _tripScheduleRepository.SaveChangesAsync(identityEntity.Email);
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
    private async Task<Trip?> ValidateTripAsync(Guid tripId, IdentityEntity identityEntity,
        Ecq110InsertTripScheduleResponse response)
    {
        var trip = await _tripRepository.Find(x => x.TripId == tripId && x.IsActive == true).FirstOrDefaultAsync();
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
    /// Validate that all dates within the trip's start and end dates are scheduled
    /// </summary>
    /// <param name="request"></param>
    /// <param name="trip"></param>
    /// <param name="response"></param>
    /// <returns></returns>
    private bool ValidateFullScheduleDates(Ecq110InsertTripScheduleRequest request, Trip trip,
        Ecq110InsertTripScheduleResponse response)
    {
        if (trip.StartDate.HasValue && trip.EndDate.HasValue)
        {
            var startDate = trip.StartDate.Value;
            var endDate = trip.EndDate.Value;
            var totalDays = (endDate.DayNumber - startDate.DayNumber) + 1;
            var allDates = Enumerable.Range(0, totalDays).Select(offset => startDate.AddDays(offset));

            var missingDates = allDates
                .Where(date => !request.TripScheduleDetails.Any(s => s.ScheduleDate == date))
                .ToList();
            if (missingDates.Any())
            {
                response.SetMessage(MessageId.I00000,
                    $"You need to schedule for the following days: {string.Join(", ", missingDates.Select(d => d.ToString("dd/MM/yyyy")))}");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Get service type mapping for trip schedule details
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    /// <returns></returns>
    private async Task<Dictionary<Guid, string>?> GetServiceTypeMappingAsync(Ecq110InsertTripScheduleRequest request,
        Ecq110InsertTripScheduleResponse response)
    {
        var serviceTypeDict = new Dictionary<Guid, string>();
        var serviceIds = request.TripScheduleDetails.Where(s => s.ServiceId != null).Select(s => s.ServiceId!.Value)
            .Distinct();
        foreach (var id in serviceIds)
        {
            var hotel = await _hotelRepository.Find(x => x.HotelId == id && x.IsActive == true).FirstOrDefaultAsync();
            if (hotel != null)
            {
                serviceTypeDict[id] = ConstantEnum.EntityType.Hotel.ToString();
                continue;
            }

            var restaurant = await _restaurantDetailRepository.Find(x => x.RestaurantId == id && x.IsActive == true)
                .FirstOrDefaultAsync();
            if (restaurant != null)
            {
                serviceTypeDict[id] = ConstantEnum.EntityType.Restaurant.ToString();
                continue;
            }

            var attraction = await _attractionDetailRepository.Find(x => x.AttractionId == id && x.IsActive == true)
                .FirstOrDefaultAsync();
            if (attraction != null)
            {
                serviceTypeDict[id] = ConstantEnum.EntityType.Attraction.ToString();
                continue;
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
    /// <returns></returns>
    private async Task<bool> ValidateTotalCostAsync(
        Ecq110InsertTripScheduleRequest request,
        Trip trip,
        Dictionary<Guid, string> serviceTypeDict,
        Ecq110InsertTripScheduleResponse response,
        Dictionary<(Guid, DateOnly), decimal> serviceCostMap)
    {
        decimal totalCost = 0;

        // Sum all custom estimated costs from user input
        var customCosts = request.TripScheduleDetails
            .Where(x => x.ServiceId == null && x.EstimatedCost.HasValue)
            .Sum(x => x.EstimatedCost.Value);
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
                var rooms = await _hotelRoomRepository.Find(hr =>
                        hr.HotelId == serviceId && hr.IsAvailable == true && hr.IsActive == true)
                    .OrderBy(r => r.PricePerNight).ToListAsync();

                int guestRemaining = trip.NumberOfPeople ?? 0;
                var selectedRooms = new List<HotelRoom>();

                foreach (var room in rooms)
                {
                    selectedRooms.Add(room);
                    guestRemaining -= room.MaxGuests;
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
                var restaurant = await _restaurantDetailRepository
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
                var attraction = await _attractionDetailRepository
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
    /// <param name="selectedRooms"></param>
    /// <returns></returns>
    private bool IsValidRoomCombination(List<HotelRoom?> availableRooms, int requiredGuests, decimal maxBudget,
        int stayNights, out List<HotelRoom> selectedRooms)
    {
        selectedRooms = new List<HotelRoom>();
        var allCombinations = GetAllRoomCombinations(availableRooms);

        foreach (var combination in allCombinations)
        {
            int totalCapacity = combination.Sum(r => r.MaxGuests);
            decimal totalCost = combination.Sum(r => r.PricePerNight * stayNights);

            if (totalCapacity >= requiredGuests && totalCost <= maxBudget)
            {
                selectedRooms = combination;
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

    public async Task<Ecq110InsertTripScheduleWithAiResponse> InsertTripScheduleUseAi(
        Ecq110InsertTripScheduleWithAiRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq110InsertTripScheduleWithAiResponse { Success = false };

        // Check if trip exists
        var trip = await _tripRepository.Find(x => x.TripId == request.TripId && x.IsActive == true)
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

        // Begin transaction
        await _tripScheduleRepository.ExecuteInTransactionAsync(async () =>
        {
            var hotelList = new List<Ecq210HotelEntity>();
            var restaurantList = new List<Ecq220RestaurantEntity>();
            var attractionList = new List<Ecq230AttractionDetailEntity>();

            var hotelSelects = _hotelRepository.GetView<VwHotel>().ToListAsync();
            var restaurantSelects = _restaurantDetailRepository.GetView<VwRestaurant>().ToListAsync();
            var attractionSelects = _attractionDetailRepository.GetView<VwAttraction>().ToListAsync();

            Task.WhenAll(hotelSelects, restaurantSelects, attractionSelects).Wait();

            foreach (var tripDestination in trip.TripDestinations)
            {
                hotelSelects.Result.Find(x => x.DestinationId == tripDestination.DestinationId);
                restaurantSelects.Result.Find(x => x.DestinationId == tripDestination.DestinationId);
                attractionSelects.Result.Find(x => x.DestinationId == tripDestination.DestinationId);

                hotelSelects.Result.ForEach(h =>
                {
                    if (h.DestinationId == tripDestination.DestinationId)
                    {
                        hotelList.Add(new Ecq210HotelEntity
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
                        });
                    }
                });

                restaurantSelects.Result.ForEach(r =>
                {
                    if (r.DestinationId == tripDestination.DestinationId)
                    {
                        restaurantList.Add(new Ecq220RestaurantEntity
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
                        });
                    }
                });

                attractionSelects.Result.ForEach(a =>
                {
                    if (a.DestinationId == tripDestination.DestinationId)
                    {
                        attractionList.Add(new Ecq230AttractionDetailEntity
                        {
                            AttractionId = a.AttractionId,
                            AttractionName = a.AttractionName,
                            PartnerId = a.PartnerId,
                            AttractionType = a.AttractionType,
                            TicketPrice = a.TicketPrice,
                            OpenTime = StringUtil.ConvertToHhMm(a.OpenTime),
                            DestinationId = a.DestinationId,
                            DestinationName = a.DestinationName,
                            PhoneNumber = a.PhoneNumber,
                            Address = a.Address,
                            GuideAvailable = a.GuideAvailable,
                            AgeLimit = a.AgeLimit,
                            DurationMinutes = a.DurationMinutes,
                            AverageRating = a.AverageRating,
                            TotalRatings = a.TotalRatings
                        });
                    }
                });
            }

            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
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
        var schedule = await _tripScheduleRepository.GetView<VwTripSchedule>(x => x.TripId == tripId)
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

        var schedules = await _tripScheduleRepository.GetView<VwTripSchedule>(x => x.UserId == userId).ToListAsync();

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
    public async Task<Ecq110UpdateTripScheduleResponse> UpdateTripSchedule(Ecq110UpdateTripScheduleRequest request,
        IdentityEntity identityEntity)
    {
        var response = new Ecq110UpdateTripScheduleResponse { Success = false };

        // Find trip by ID
        var trip = await _tripRepository.Find(x => x.TripId == request.TripId && x.IsActive == true)
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

        // // Check if schedule date is within trip date range
        // if (request.ScheduleDate < trip.StartDate || request.ScheduleDate > trip.EndDate)
        // {
        //     response.SetMessage(MessageId.I00000, "Schedule date must be within trip date range");
        //     return response;
        // }

        // Find schedule by ID
        var schedule = await _tripScheduleRepository.Find(x => x.ScheduleId == request.ScheduleId && x.IsActive == true)
            .FirstOrDefaultAsync();
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
        await _tripScheduleRepository.UpdateAsync(schedule);
        await _tripScheduleRepository.SaveChangesAsync(identityEntity.Email);

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
    public async Task<Ecq110DeleteTripScheduleResponse> DeleteTripSchedule(Ecq110DeleteTripScheduleRequest request,
        IdentityEntity identityEntity)
    {
        var response = new Ecq110DeleteTripScheduleResponse { Success = false };

        // Find schedule by ID
        var schedule = await _tripScheduleRepository.Find(x => x.ScheduleId == request.ScheduleId && x.IsActive == true)
            .FirstOrDefaultAsync();
        if (schedule == null)
        {
            response.SetMessage(MessageId.I00000, "Trip schedule not found");
            return response;
        }

        // Find trip to verify ownership
        var trip = await _tripRepository.Find(x => x.TripId == schedule.TripId && x.IsActive == true)
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

        // Soft delete the schedule
        await _tripScheduleRepository.UpdateAsync(schedule);
        await _tripScheduleRepository.SaveChangesAsync(identityEntity.Email, true);

        // Set response
        response.Response = true;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }
}