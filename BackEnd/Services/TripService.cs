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

public class TripService : ITripService
{
    private readonly IBaseRepository<Trip, Guid> _tripRepository;
    private readonly IBaseRepository<TripDestination, Guid> _tripDestinationRepository;
    private readonly IBaseRepository<TripSchedule, Guid> _tripScheduleRepository;
    private readonly IBaseRepository<AttractionDetail, Guid> _attractionDetailRepository;
    private readonly IBaseRepository<RestaurantDetail, Guid> _restaurantDetailRepository;
    private readonly IBaseRepository<Hotel, Guid> _hotelRepository;

    public TripService(IBaseRepository<Trip, Guid> tripRepository,
        IBaseRepository<TripDestination, Guid> tripDestinationRepository,
        IBaseRepository<TripSchedule, Guid> tripScheduleRepository,
        IBaseRepository<AttractionDetail, Guid> attractionDetailRepository,
        IBaseRepository<RestaurantDetail, Guid> restaurantDetailRepository,
        IBaseRepository<Hotel, Guid> hotelRepository)
    {
        _tripRepository = tripRepository;
        _tripDestinationRepository = tripDestinationRepository;
        _tripScheduleRepository = tripScheduleRepository;
        _attractionDetailRepository = attractionDetailRepository;
        _restaurantDetailRepository = restaurantDetailRepository;
        _hotelRepository = hotelRepository;
    }

    /// <summary>
    /// Select a trip by ID
    /// </summary>
    /// <param name="tripId"></param>
    /// <returns></returns>
    public async Task<Ecq110SelectTripResponse> SelectTrip(Guid tripId)
    {
        var response = new Ecq110SelectTripResponse { Success = false };

        // Select trip by ID
        var trip = await _tripRepository.GetView<VwTrip>(x => x.TripId == tripId)
            .Select(x => new Ecq110TripEntity
            {
                TripId = x.TripId,
                UserId = x.UserId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                TripName = x.TripName!,
                Description = x.Description,
                StartDate = StringUtil.ConvertToDateAsDdMmYyyy(x.StartDate),
                EndDate = StringUtil.ConvertToDateAsDdMmYyyy(x.EndDate),
                NumberOfPeople = x.NumberOfPeople,
                TotalEstimatedCost = x.TotalEstimatedCost,
                Status = x.Status,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.UpdatedAt),
            }).FirstOrDefaultAsync();
        if (trip == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.TripNotFound);
            return response;
        }

        response.Response = trip;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }

    /// <summary>
    /// Select all trips
    /// </summary>
    /// <returns></returns>
    public async Task<Ecq110SelectTripsResponse> SelectTrips()
    {
        var response = new Ecq110SelectTripsResponse { Success = false };

        // Select all trips
        response.Response = await _tripRepository.GetView<VwTrip>()
            .Select(x => new Ecq110TripListEntity
            {
                TripId = x.TripId,
                UserId = x.UserId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                TripName = x.TripName!,
                Description = x.Description,
                StartDate = StringUtil.ConvertToDateAsDdMmYyyy(x.StartDate),
                EndDate = StringUtil.ConvertToDateAsDdMmYyyy(x.EndDate),
                NumberOfPeople = x.NumberOfPeople ?? 0,
                TotalEstimatedCost = x.TotalEstimatedCost,
                Status = x.Status,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.CreatedAt),
            }).ToListAsync();

        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }

    /// <summary>
    /// Insert a new trip
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq110InsertTripResponse> InsertTrip(Ecq110InsertTripRequest request,
        IdentityEntity identityEntity)
    {
        var response = new Ecq110InsertTripResponse { Success = false };

        // Begin transaction
        await _tripRepository.ExecuteInTransactionAsync(async () =>
        {
            var tripId = Guid.NewGuid();
            var trip = new Trip
            {
                TripId = tripId,
                UserId = Guid.Parse(identityEntity.UserId),
                TripName = request.TripName,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                NumberOfPeople = request.NumberOfPeople,
                TotalEstimatedCost = request.TotalEstimatedCost,
                Description = request.Description,
                Status = (byte)ConstantEnum.TripStatus.Planned,
                StartingPointAddress = request.StartingPointAddress ?? identityEntity.Address,
            };

            // Save to repository
            await _tripRepository.AddAsync(trip);
            await _tripRepository.SaveChangesAsync(identityEntity.Email);

            var orderIndex = 1;
            foreach (var destination in request.Destinations)
            {
                var tripDestination = new TripDestination
                {
                    TripId = tripId,
                    DestinationId = destination.DestinationId,
                    OrderIndex = orderIndex,
                    Note = destination.Note
                };
                await _tripDestinationRepository.AddAsync(tripDestination);
                orderIndex++;
            }

            await _tripDestinationRepository.SaveChangesAsync(identityEntity.Email);

            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }

    /// <summary>
    /// Update an existing trip
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq110UpdateTripResponse> UpdateTrip(Ecq110UpdateTripRequest request,
        IdentityEntity identityEntity)
    {
        var response = new Ecq110UpdateTripResponse { Success = false };

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

        // Update trip properties
        trip.TripName = request.TripName;
        trip.StartDate = request.StartDate;
        trip.EndDate = request.EndDate;
        trip.NumberOfPeople = request.NumberOfPeople;
        trip.TotalEstimatedCost = request.TotalEstimatedCost;
        trip.Description = request.Description;
        trip.Status = request.Status;

        // Save changes
        await _tripRepository.UpdateAsync(trip);
        await _tripRepository.SaveChangesAsync(identityEntity.Email);

        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }

    /// <summary>
    /// Delete a trip (soft delete)
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq110DeleteTripResponse> DeleteTrip(Ecq110DeleteTripRequest request,
        IdentityEntity identityEntity)
    {
        var response = new Ecq110DeleteTripResponse { Success = false };

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

        // Save changes with soft delete flag
        await _tripRepository.UpdateAsync(trip);
        await _tripRepository.SaveChangesAsync(identityEntity.Email, true);

        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
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

        // Check if trip exists
        var trip = await _tripRepository.Find(x => x.TripId == request.TripId && x.IsActive == true).FirstOrDefaultAsync();
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
            foreach (var detail in request.TripScheduleDetails)
            {
                if (detail.ScheduleDate < trip.StartDate || detail.ScheduleDate > trip.EndDate)
                {
                    response.SetMessage(MessageId.I00000, "Schedule date cannot be before trip start date or after trip end date.");
                    return false;
                }

                Guid? serviceId = null;
                string? serviceType = null;

                if (detail.ServiceId != null)
                {
                    var hotel = await _hotelRepository.Find(x => x.HotelId == detail.ServiceId && x.IsActive == true).FirstOrDefaultAsync();
                    if (hotel != null)
                    {
                        serviceId = hotel.HotelId;
                        serviceType = ConstantEnum.EntityType.Hotel.ToString();
                    }
                    else
                    {
                        var restaurant = await _restaurantDetailRepository.Find(x => x.RestaurantId == detail.ServiceId && x.IsActive == true).FirstOrDefaultAsync();
                        if (restaurant != null)
                        {
                            serviceId = restaurant.RestaurantId;
                            serviceType = ConstantEnum.EntityType.Restaurant.ToString();
                        }
                        else
                        {
                            var attraction = await _attractionDetailRepository.Find(x => x.AttractionId == detail.ServiceId && x.IsActive == true).FirstOrDefaultAsync();
                            if (attraction != null)
                            {
                                serviceId = attraction.AttractionId;
                                serviceType = ConstantEnum.EntityType.Attraction.ToString();
                            }
                            else
                            {
                                response.SetMessage(MessageId.I00000, "Service not found.");
                                return false;
                            }
                        }
                    }
                }
                
                var latLong = await GetLatLongUsingNominatimAsync(detail.Address);
                
                // Insert new trip schedule
                var tripSchedule = new TripSchedule
                {
                    TripId = request.TripId,
                    ScheduleDate = detail.ScheduleDate,
                    Title = detail.Title,
                    Description = detail.Description,
                    StartTime = detail.StartTime,
                    EndTime = detail.EndTime,
                    Location = latLong != null ? $"{latLong.Value.lat},{latLong.Value.lng}" : null,
                };
                
                // If serviceId and serviceType are set, assign them to the trip schedule
                if (serviceId.HasValue && serviceType != null)
                {
                    tripSchedule.ServiceId = serviceId.Value;
                    tripSchedule.ServiceType = serviceType;
                }

                await _tripScheduleRepository.AddAsync(tripSchedule);
            }

            // Save to repository
            await _tripScheduleRepository.SaveChangesAsync(identityEntity.Email);

            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });

        return response;
    }

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
                Address = x.Address!,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.UpdatedAt)
            })
            .ToListAsync();

        // True
        response.Response = schedule;
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

        // Find Trip by userId
        var trip = await _tripRepository.GetView<VwTrip>(x => x.UserId == userId).ToListAsync();
        foreach (var schedule in trip)
        {
        }

        var schedules = await _tripScheduleRepository.GetView<VwTripSchedule>(x => x.TripId == userId)
            .Select(x => new Ecq110TripScheduleEntity
            {
                ScheduleId = x.ScheduleId,
                TripId = x.TripId,
                ScheduleDate = StringUtil.ConvertToDateAsDdMmYyyy(x.ScheduleDate),
                Title = x.ScheduleTitle,
                Description = x.ScheduleDescription!,
                StartTime = StringUtil.ConvertToHhMm(x.StartTime),
                EndTime = StringUtil.ConvertToHhMm(x.EndTime),
                Address = x.Address!,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.UpdatedAt)
            })
            .OrderBy(x => x.ScheduleDate)
            .ThenBy(x => x.StartTime)
            .ToListAsync();

        // Set response
        response.Response = schedules;
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
    
    public async Task<(double lat, double lng)?> GetLatLongUsingNominatimAsync(string address)
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
}