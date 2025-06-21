using BackEnd.DTOs.Ecq110;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IBookingService
{
    Task<Ecq110InsertBookingTripResponse> InsertBookingTrip(Ecq110InsertBookingTripRequest request, IdentityEntity identityEntity);
}