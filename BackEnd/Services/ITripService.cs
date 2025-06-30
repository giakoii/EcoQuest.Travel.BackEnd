using BackEnd.DTOs.Ecq110;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface ITripService
{
    Task<Ecq110SelectTripResponse> SelectTrip(Guid tripId);
    
    Task<Ecq110SelectTripsResponse> SelectTrips(IdentityEntity identityEntity);
    
    Task<Ecq110InsertTripResponse> InsertTrip(Ecq110InsertTripRequest request, IdentityEntity identityEntity);
    
    Task<Ecq110UpdateTripResponse> UpdateTrip(Ecq110UpdateTripRequest request, IdentityEntity identityEntity);
    
    Task<Ecq110DeleteTripResponse> DeleteTrip(Ecq110DeleteTripRequest request, IdentityEntity identityEntity);
}
