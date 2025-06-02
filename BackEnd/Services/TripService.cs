using BackEnd.DTOs.Trip;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;

namespace BackEnd.Services;

public class TripService : ITripService
{
    private readonly IBaseRepository<Trip, Guid> _tripRepository;
    
    public TripService(IBaseRepository<Trip, Guid> tripRepository)
    {
        _tripRepository = tripRepository;
    }

    /// <summary>
    /// InsertTrip - Creates a new trip for the user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<InsertTripResponse> InsertTrip(InsertTripRequest request, IdentityEntity identityEntity)
    {
        var response = new InsertTripResponse { Success = false };
        
        // Begin transaction
        await _tripRepository.ExecuteInTransactionAsync(async () =>
        {
            var newTrip = new Trip
            {
                TripName = request.TripName,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                NumberOfPeople = request.NumberOfPeople,
                TotalEstimatedCost = request.TotalEstimatedCost,
                UserId = Guid.Parse(identityEntity.UserId),
            };
            _tripRepository.Add(newTrip);
            await _tripRepository.SaveChangesAsync(identityEntity.Email);
            
            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }

    public SelectTripResponse SelectTrip(SelectTripRequest request, IdentityEntity identityEntity)
    {
        throw new NotImplementedException();
    }
}