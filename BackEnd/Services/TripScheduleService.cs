using BackEnd.DTOs.Ecq110;
using BackEnd.DTOs.Ecq200;
using BackEnd.DTOs.Ecq210;
using BackEnd.DTOs.Ecq211;
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

        // Check if trip is already paid (prevent booking modification after payment)
        var payment = await _repositoryWrapper.PaymentRepository
            .Find(x => x.TripId == request.TripId && x.IsActive &&
                       x.Status != nameof(ConstantEnum.PaymentStatus.Cancelled))
            .FirstOrDefaultAsync();
        if (payment != null)
        {
            response.SetMessage(MessageId.I00000, "Cannot modify trip schedule. Trip is already paid.");
            return response;
        }

        // Get service type mapping
        var serviceTypeDict = await GetServiceTypeMappingAsync(request);
        
        // Prepare to cache estimated cost per service per date
        var serviceCostMap = new Dictionary<(Guid, DateOnly), decimal>();
        
        // Insert trip schedule into DB
        await _repositoryWrapper.TripScheduleRepository.ExecuteInTransactionAsync(async () =>
        {
            // Create list to store all created trip schedules with services for booking creation
            var schedulesWithServices = new List<TripSchedule>();

            foreach (var detail in request.TripScheduleDetails)
            {
                // Validate schedule date is within trip date range
                if (detail.ScheduleDate < trip.StartDate || detail.ScheduleDate > trip.EndDate)
                {
                    response.SetMessage(MessageId.I00000, "Schedule date cannot be before trip start date or after trip end date.");
                    return false;
                }

                var serviceId = detail.ServiceId;
                string? serviceType = null;
                decimal? estimatedCost = detail.EstimatedCost;

                // Process service-related information
                if (serviceId.HasValue)
                {
                    // Get service type from mapping
                    serviceTypeDict!.TryGetValue(serviceId.Value, out serviceType);
                    
                    // Ưu tiên chi phí từ serviceCostMap (đã được validated trong ValidateTotalCostAsync)
                    // Nếu không có trong map, giữ nguyên chi phí từ AI (detail.EstimatedCost)
                    if (serviceCostMap.TryGetValue((serviceId.Value, detail.ScheduleDate), out var mappedCost))
                    {
                        estimatedCost = mappedCost;
                    }
                }
                // Else: Không có serviceId, giữ nguyên detail.EstimatedCost (custom activities)

                // Get coordinates for the address using Nominatim API
                var latLong = await GetLatLongUsingNominatimAsync(detail.Address);

                // Create trip schedule entity
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

                // Add to repository
                await _repositoryWrapper.TripScheduleRepository.AddAsync(tripSchedule);

                // Collect schedules with services for booking creation
                if (serviceId.HasValue)
                {
                    schedulesWithServices.Add(tripSchedule);
                }
            }

            // Save trip schedule changes first
            await _repositoryWrapper.TripScheduleRepository.SaveChangesAsync(identityEntity.Email);

            // Create bookings for schedules with services
            if (schedulesWithServices.Any())
            {
                await CreateBookingsFromSchedules(schedulesWithServices, trip, identityEntity);
            }
            
            // Set success response
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });

        return response;
    }

    /// <summary>
    /// Create bookings from trip schedules with services
    /// </summary>
    /// <param name="schedulesWithServices"></param>
    /// <param name="trip"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    private async Task CreateBookingsFromSchedules(List<TripSchedule> schedulesWithServices, Trip trip, IdentityEntity identityEntity)
    {
        // Group schedules by service type for different booking logic
        var hotelSchedules = schedulesWithServices
            .Where(x => x.ServiceType == nameof(ConstantEnum.EntityType.Hotel))
            .ToList();

        var otherServiceSchedules = schedulesWithServices
            .Where(x => x.ServiceType != nameof(ConstantEnum.EntityType.Hotel))
            .ToList();

        // Handle hotel bookings (group by hotel ID)
        if (hotelSchedules.Any())
        {
            var hotelGroups = hotelSchedules.GroupBy(x => x.ServiceId);

            foreach (var group in hotelGroups)
            {
                var hotelId = group.Key!.Value;
                var schedulesForHotel = group.ToList();
                var checkinDate = schedulesForHotel.Min(x => x.ScheduleDate);

                // If only one schedule, set checkout to trip end date + 1, else use max schedule date + 1
                var checkoutDate = trip.EndDate!.Value.AddDays(1);
                var booking = new Booking
                {
                    BookingId = Guid.NewGuid(),
                    TripId = trip.TripId,
                    UserId = trip.UserId,
                    ServiceId = hotelId,
                    ServiceType = nameof(ConstantEnum.EntityType.Hotel),
                    ScheduleDate = checkinDate,
                    NumberOfGuests = trip.NumberOfPeople ?? 1,
                    Status = nameof(ConstantEnum.BookingStatus.Pending),
                };

                var hotelBookings = new List<HotelBooking>();
                decimal totalCost = 0;

                foreach (var schedule in schedulesForHotel)
                {
                    // Each schedule represents a hotel room booking
                    var nights = (checkoutDate.DayNumber - checkinDate.DayNumber);
                    if (nights < 1) nights = 1;

                    int requiredRooms = 1; // Assume 1 room per schedule

                    var hotelBooking = new HotelBooking
                    {
                        HotelBookingId = Guid.NewGuid(),
                        BookingId = booking.BookingId,
                        RoomId = schedule.ServiceId!.Value,
                        CheckinDate = checkinDate,
                        CheckoutDate = checkoutDate,
                        NumberOfRooms = requiredRooms,
                        PricePerNight = schedule.EstimatedCost ?? 0,
                    };

                    totalCost += hotelBooking.PricePerNight * nights;
                    hotelBookings.Add(hotelBooking);
                }

                booking.TotalCost = totalCost;
                booking.HotelBookings = hotelBookings;
                await _repositoryWrapper.BookingRepository.AddAsync(booking);
            }
        }

        // Handle other service bookings (restaurant, attraction, etc.)
        foreach (var schedule in otherServiceSchedules)
        {
            var booking = new Booking
            {
                BookingId = Guid.NewGuid(),
                UserId = Guid.Parse(identityEntity.UserId),
                TripId = trip.TripId,
                ServiceId = schedule.ServiceId,
                ServiceType = schedule.ServiceType,
                ScheduleDate = schedule.ScheduleDate,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                TotalCost = schedule.EstimatedCost ?? 0,
                NumberOfGuests = trip.NumberOfPeople ?? 1,
                Status = nameof(ConstantEnum.BookingStatus.Pending),
            };
            await _repositoryWrapper.BookingRepository.AddAsync(booking);
        }

        // Save all booking changes
        await _repositoryWrapper.BookingRepository.SaveChangesAsync(identityEntity.Email);
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
        var trip = await _repositoryWrapper.TripRepository.Find(x => x.TripId == tripId && x.IsActive == true)
            .FirstOrDefaultAsync();
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
        var serviceIds = request.TripScheduleDetails
            .Where(s => s.ServiceId != null)
            .Select(s => s.ServiceId!.Value)
            .Distinct();
        foreach (var id in serviceIds)
        {
            var hotelRoom = await _repositoryWrapper.HotelRoomRepository.Find(x => x.RoomId == id && x.IsActive == true).FirstOrDefaultAsync();
            if (hotelRoom != null)
            {
                serviceTypeDict[id] = nameof(ConstantEnum.EntityType.Hotel);
                continue;
            }

            var restaurant = await _repositoryWrapper.RestaurantDetailRepository
                .Find(x => x.RestaurantId == id && x.IsActive == true)
                .FirstOrDefaultAsync();
            if (restaurant != null)
            {
                serviceTypeDict[id] = nameof(ConstantEnum.EntityType.Restaurant);
                continue;
            }

            var attraction = await _repositoryWrapper.AttractionDetailRepository
                .Find(x => x.AttractionId == id && x.IsActive == true)
                .FirstOrDefaultAsync();
            if (attraction != null)
            {
                serviceTypeDict[id] = nameof(ConstantEnum.EntityType.Attraction);
            }
        }

        return serviceTypeDict;
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
        var userId = Guid.Parse(identityEntity.UserId);
        try
        {
            var user = await _repositoryWrapper.UserRepository.Find(x => x.UserId == userId && x.IsActive == true)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                response.SetMessage(MessageId.I00000, "User not found or inactive");
                return response;
            }

            // If the user account is not a premium account, return error
            if (user.UserType != (byte)ConstantEnum.UserType.Premier)
            {
                response.SetMessage(MessageId.I00000, "You must have a premium account to use AI scheduling.");
                return response;
            }

            // Check if trip exists
            var trip = await _repositoryWrapper.TripRepository
                .Find(x => x.TripId == request.TripId && x.IsActive == true, false, x => x.TripDestinations)
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

            // Get hotel rooms for the hotels in destinations
            var hotelIds = hotels.Select(h => h.HotelId).ToList();
            var hotelRooms = new List<VwHotelRoom>();
            if (hotelIds.Any())
            {
                hotelRooms = await _repositoryWrapper.HotelRoomRepository.GetView<VwHotelRoom>()
                    .Where(hr => hotelIds.Contains(hr.HotelId) && hr.IsAvailable == true && hr.IsActive == true)
                    .ToListAsync();
            }

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

            // Convert hotel rooms to AI entities
            var hotelRoomList = hotelRooms.Select(hr => new Ecq211HotelRoomEntity
            {
                RoomId = hr.RoomId,
                HotelId = hr.HotelId,
                RoomType = hr.RoomType,
                Description = hr.Description!,
                MaxGuests = hr.MaxGuests,
                PricePerNight = hr.PricePerNight,
                IsAvailable = hr.IsAvailable,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(hr.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(hr.UpdatedAt),
                HotelRoomImages = new List<string?>() // Images will be loaded separately if needed
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

            // Build detailed hotel information from hotel rooms with hotel details
            var hotelRoomInfoWithHotels = hotelRoomList
                .Where(hr => hotelList.Any(h => h.HotelId == hr.HotelId)) // Đảm bảo hotel room có khách sạn tương ứng
                .Select(hr =>
                {
                    var hotel = hotelList.First(h =>
                        h.HotelId == hr.HotelId); // Dùng First thay vì FirstOrDefault vì đã filter ở trên
                    return $"- {hr.RoomType}: Giá {hr.PricePerNight:C}/đêm, Sức chứa tối đa: {hr.MaxGuests} người, " +
                           $"Mô tả: {hr.Description}, Tình trạng: {(hr.IsAvailable == true ? "Có sẵn" : "Không có sẵn")}, " +
                           $"Khách sạn: {hotel.Name}, Địa chỉ: {hotel.AddressLine}, {hotel.District}, {hotel.Province}, HotelRoomId: {hr.RoomId}";
                });

            var hotelRoomInfo = hotelRoomInfoWithHotels.Any()
                ? "**Phòng khách sạn:**\n" + string.Join("\n", hotelRoomInfoWithHotels)
                : "Không có phòng khách sạn khả dụng";

            // Call AI to generate schedule
            var generateSchedule = await _aiLogic.AskAiModel(
                hotelList,
                hotelRoomList,
                attractionList,
                restaurantList,
                destinationList,
                startDateStr,
                endDateStr,
                numberOfPeopleStr,
                totalEstimatedCostStr,
                hotelRoomInfo); // Pass the formatted hotel room info

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

        var schedules = await _repositoryWrapper.TripScheduleRepository.GetView<VwTripSchedule>(x => x.UserId == userId)
            .ToListAsync();

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
        var trip = await _repositoryWrapper.TripRepository.Find(x => x.TripId == request.TripId && x.IsActive == true)
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
        var schedule = await _repositoryWrapper.TripScheduleRepository
            .Find(x => x.ScheduleId == request.ScheduleId && x.IsActive).FirstOrDefaultAsync();
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
    public async Task<Ecq110DeleteTripScheduleResponse> DeleteTripSchedule(Ecq110DeleteTripScheduleRequest request,
        IdentityEntity identityEntity)
    {
        var response = new Ecq110DeleteTripScheduleResponse { Success = false };

        // Find schedule by ID
        var schedule = await _repositoryWrapper.TripScheduleRepository
            .Find(x => x.ScheduleId == request.ScheduleId && x.IsActive).FirstOrDefaultAsync();
        if (schedule == null)
        {
            response.SetMessage(MessageId.I00000, "Trip schedule not found");
            return response;
        }

        // Find trip to verify ownership
        var trip = await _repositoryWrapper.TripRepository.Find(x => x.TripId == schedule.TripId && x.IsActive == true)
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
        if (request.ServiceType == ((int)ConstantEnum.ServiceType.Hotel))
        {
            // Get hotels
            var hotels = await _repositoryWrapper.HotelRepository.GetView<VwHotel>()
                .Select(x => new Ecq110SelectServiceEntity
                {
                    ServiceId = x.HotelId,
                    ServiceName = x.HotelName,
                    Address = x.Address!,
                    Cost = $"{x.MinPrice} - {x.MaxPrice}",
                    DestinationId = x.DestinationId!.Value
                }).ToListAsync();
            if (request.DestinationIds != null)
            {
                hotels = hotels
                    .Where(h => request.DestinationIds.Contains(h.DestinationId!))
                    .ToList();
            }
            
            entityResponse = hotels;
        }
        else if (request.ServiceType == ((int)ConstantEnum.ServiceType.Restaurant))
        {
            // Get restaurants
            var restaurants = await _repositoryWrapper.RestaurantDetailRepository.GetView<VwRestaurant>()
                .Select(x => new Ecq110SelectServiceEntity
                {
                    ServiceId = x.RestaurantId,
                    ServiceName = x.RestaurantName!,
                    Address = x.Address!,
                    Cost = $"{x.MinPrice} - {x.MaxPrice}",
                    DestinationId = x.DestinationId!.Value
                }).ToListAsync();
            
            if (request.DestinationIds != null)
            {
                restaurants = restaurants
                    .Where(r => request.DestinationIds.Contains(r.DestinationId!))
                    .ToList();
            }
            entityResponse = restaurants;
        }
        else if (request.ServiceType == ((int)ConstantEnum.ServiceType.Attraction))
        {
            // Get attractions
            var attractions = await _repositoryWrapper.AttractionDetailRepository.GetView<VwAttraction>()
                .Select(x => new Ecq110SelectServiceEntity
                {
                    ServiceId = x.AttractionId,
                    ServiceName = x.AttractionName!,
                    Address = x.Address!,
                    Cost = $"{x.TicketPrice}",
                    DestinationId = x.DestinationId!.Value
                }).ToListAsync();
            if (request.DestinationIds != null)
            {
                attractions = attractions
                    .Where(a => request.DestinationIds.Contains(a.DestinationId!))
                    .ToList();
            }
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
