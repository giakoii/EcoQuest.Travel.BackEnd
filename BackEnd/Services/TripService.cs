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
    public InsertTripResponse InsertTrip(InsertTripRequest request, IdentityEntity identityEntity)
    {
        var response = new InsertTripResponse { Success = false };
        
        // Begin transaction
        _tripRepository.ExecuteInTransaction((() =>
        {
            var newTrip = new Trip
            {
                TripName = request.TripName,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                NumberOfPeople = request.NumberOfPeople,
                TotalEstimatedCost = request.TotalEstimatedCost,
                UserId = identityEntity.UserId,
            };
            _tripRepository.Add(newTrip);
            _tripRepository.SaveChanges(identityEntity.Email);
            
            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        }));
        return response;
    }

    public SelectTripResponse SelectTrip(SelectTripRequest request, IdentityEntity identityEntity)
    {
        throw new NotImplementedException();
    }
}