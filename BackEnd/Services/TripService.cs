using BackEnd.DTOs.Ecq110;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.SystemClient;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services;

public class TripService : ITripService
{
    private readonly IBaseRepository<Trip, Guid> _tripRepository;
    private readonly IBaseRepository<TripDestination, Guid> _tripDestinationRepository;
    private readonly IBaseRepository<TripSchedule, Guid> _tripScheduleRepository;
    private readonly IBaseRepository<AttractionDetail, Guid> _attractionDetailRepository;
    private readonly IBaseRepository<RestaurantDetail, Guid> _restaurantDetailRepository;
    private readonly IBaseRepository<Hotel, Guid> _hotelRepository;
    private readonly IBaseRepository<HotelRoom, Guid> _hotelRoomRepository;

    public TripService(IBaseRepository<Trip, Guid> tripRepository, IBaseRepository<TripDestination, Guid> tripDestinationRepository,
        IBaseRepository<TripSchedule, Guid> tripScheduleRepository, IBaseRepository<AttractionDetail, Guid> attractionDetailRepository,
        IBaseRepository<RestaurantDetail, Guid> restaurantDetailRepository, IBaseRepository<Hotel, Guid> hotelRepository, IBaseRepository<HotelRoom, Guid> hotelRoomRepository)
    {
        _tripRepository = tripRepository;
        _tripDestinationRepository = tripDestinationRepository;
        _tripScheduleRepository = tripScheduleRepository;
        _attractionDetailRepository = attractionDetailRepository;
        _restaurantDetailRepository = restaurantDetailRepository;
        _hotelRepository = hotelRepository;
        _hotelRoomRepository = hotelRoomRepository;
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
    public async Task<Ecq110SelectTripsResponse> SelectTrips(IdentityEntity identityEntity)
    {
        var response = new Ecq110SelectTripsResponse { Success = false };

        // Select all trips
        response.Response = await _tripRepository.GetView<VwTrip>(x => x.UserId == Guid.Parse(identityEntity.UserId))
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
    public async Task<Ecq110InsertTripResponse> InsertTrip(Ecq110InsertTripRequest request, IdentityEntity identityEntity)
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
    
}