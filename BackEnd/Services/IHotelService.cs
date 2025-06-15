using BackEnd.DTOs.Ecq210;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IHotelService
{
    Task<Ecq210InsertHotelResponse> InsertHotel(Ecq210InsertHotelRequest request, IdentityEntity identityEntity);
    
    Task<Ecq210SelectHotelResponse> SelectHotel(Ecq210SelectHotelRequest request);
    
    Task<Ecq210SelectHotelsResponse> SelectHotels();
    
    Task<Ecq210UpdateHotelResponse> UpdateHotel(Ecq210UpdateHotelRequest request, IdentityEntity identityEntity);
}
