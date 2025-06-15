using BackEnd.DTOs.Ecq220;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IRestaurantService
{
    Task<Ecq220SelectRestaurantResponse> Ecq100SelectRestaurant(Guid restaurantId);
    
    Task<Ecq220SelectRestaurantResponse> Ecq220SelectRestaurant(Guid restaurantId, IdentityEntity identityEntity);
    
    Task<Ecq220SelectRestaurantsResponse> Ecq100SelectRestaurants();
    
    Task<Ecq220SelectRestaurantsResponse> Ecq220SelectRestaurants(IdentityEntity identityEntity);
    
    Task<Ecq220InsertRestaurantResponse> InsertRestaurant(Ecq220InsertRestaurantRequest request, IdentityEntity identityEntity);
    
    Task<Ecq220UpdateRestaurantResponse> UpdateRestaurant(Ecq220UpdateRestaurantRequest request, IdentityEntity identityEntity);
}
