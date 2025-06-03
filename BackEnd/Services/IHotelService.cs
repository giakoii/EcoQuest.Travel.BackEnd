using BackEnd.DTOs.Ecq210;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IHotelService
{
    Task<Ecq210InsertHotelResponse> InsertHotel(Ecq210InsertHotelRequest request, IdentityEntity identityEntity);
}