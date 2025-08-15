using BackEnd.DTOs.Ecq100;
using BackEnd.DTOs.Ecq210;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IHotelService
{
    Task<Ecq210InsertHotelResponse> InsertHotel(Ecq210InsertHotelRequest request, IdentityEntity identityEntity);
    
    Task<Ecq210SelectHotelResponse> Ecq100SelectHotel(Ecq210SelectHotelRequest request);
    
    Task<Ecq100SelectHotelsResponse> Ecq100SelectHotels(Ecq100SelectHotelsRequest request);
    
    Task<Ecq210SelectHotelsResponse> Ecq210SelectHotels(IdentityEntity identityEntity);

    Task<Ecq210SelectHotelResponse> Ecq210SelectHotel(Ecq210SelectHotelRequest request, IdentityEntity identityEntity);
    
    Task<Ecq210UpdateHotelResponse> UpdateHotel(Ecq210UpdateHotelRequest request, IdentityEntity identityEntity);
}
