using BackEnd.DTOs.Ecq220;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IRestaurantService
{
    Task<Ecq220SelectRestaurantResponse> SelectRestaurant(Guid restaurantId);
    
    Task<Ecq220SelectRestaurantsResponse> SelectRestaurants();
}
