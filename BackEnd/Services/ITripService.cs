using BackEnd.DTOs.Trip;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface ITripService
{
    InsertTripResponse InsertTrip(InsertTripRequest request, IdentityEntity identityEntity);
    SelectTripResponse SelectTrip(SelectTripRequest request, IdentityEntity identityEntity);
}