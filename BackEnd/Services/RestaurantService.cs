using BackEnd.DTOs.Ecq220;
using BackEnd.Logics;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services;

public class RestaurantService : IRestaurantService
{
    private readonly IBaseRepository<RestaurantDetail, Guid> _restaurantRepository;
    private readonly IBaseRepository<Image, Guid> _imageRepository;
    private readonly CloudinaryLogic _cloudinary;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="restaurantRepository"></param>
    /// <param name="imageRepository"></param>
    /// <param name="cloudinary"></param>
    public RestaurantService(IBaseRepository<RestaurantDetail, Guid> restaurantRepository, 
                          IBaseRepository<Image, Guid> imageRepository,
                          CloudinaryLogic cloudinary)
    {
        _restaurantRepository = restaurantRepository;
        _imageRepository = imageRepository;
        _cloudinary = cloudinary;
    }

    /// <summary>
    /// Select a specific restaurant by ID
    /// </summary>
    /// <param name="restaurantId"></param>
    /// <returns></returns>
    public async Task<Ecq220SelectRestaurantResponse> SelectRestaurant(Guid restaurantId)
    {
        var response = new Ecq220SelectRestaurantResponse { Success = false };
        
        // Get restaurant by ID with rating information
        var restaurant = await _restaurantRepository.GetView<VwRestaurant>(r => r.RestaurantId == restaurantId)
            .Select(r => new Ecq220RestaurantDetailEntity
            {
                RestaurantId = r.RestaurantId,
                RestaurantName = r.RestaurantName!,
                OpenTime = StringUtil.ConvertToHhMm(r.OpenTime),
                CloseTime = StringUtil.ConvertToHhMm(r.CloseTime),
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(r.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(r.UpdatedAt),
                MinPrice = r.MinPrice,
                MaxPrice = r.MaxPrice,
                DestinationId = r.DestinationId,
                DestinationName = r.DestinationName,
                AverageRating = r.AverageRating,
                TotalRatings = r.TotalRatings,
                HasVegetarian = r.HasVegetarian,
                Address = r.Address,
                CuisineType = r.CuisineType,
                PartnerId = r.PartnerId,
                PhoneNumber = r.PhoneNumber,
                UpdatedBy = r.UpdatedBy,
            })
            .FirstOrDefaultAsync();
            
        if (restaurant == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.RestaurantNotFound);
            return response;
        }
            
        // Fetch restaurant images
        restaurant.RestaurantImages = (await _imageRepository
            .GetView<VwImage>(img => img.EntityId == restaurant.RestaurantId && img.EntityType == ConstantEnum.EntityImage.Restaurant.ToString())
            .Select(img => img.ImageUrl)
            .ToListAsync())!;
        
        // True
        response.Response = restaurant;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        
        return response;
    }

    /// <summary>
    /// Select all restaurants
    /// </summary>
    /// <returns></returns>
    public async Task<Ecq220SelectRestaurantsResponse> SelectRestaurants()
    {
        var response = new Ecq220SelectRestaurantsResponse { Success = false };
        
        // Select restaurants with rating information
        var restaurants = await _restaurantRepository.GetView<VwRestaurant>()
            .Select(r => new Ecq220RestaurantEntity
            {
                RestaurantId = r.RestaurantId,
                RestaurantName = r.RestaurantName!,
                AddressLine = r.Address,
                OpenTime = StringUtil.ConvertToHhMm(r.OpenTime),
                CloseTime = StringUtil.ConvertToHhMm(r.CloseTime),
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(r.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(r.UpdatedAt),
                MinPrice = r.MinPrice,
                MaxPrice = r.MaxPrice,
                DestinationId = r.DestinationId,
                DestinationName = r.DestinationName,
                AverageRating = r.AverageRating,
                TotalRatings = r.TotalRatings
            })
            .ToListAsync();
        
        // Fetch images for each restaurant
        foreach (var restaurant in restaurants)
        {
            restaurant.RestaurantImages = (await _imageRepository
                .GetView<VwImage>(img => img.EntityId == restaurant.RestaurantId && img.EntityType == ConstantEnum.EntityImage.Restaurant.ToString())
                .Select(img => img.ImageUrl)
                .ToListAsync())!;
        }
        
        // True
        response.Response = restaurants;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        
        return response;
    }
}
