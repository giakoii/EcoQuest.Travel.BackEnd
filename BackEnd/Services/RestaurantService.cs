using BackEnd.DTOs.Ecq220;
using BackEnd.Logics;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.SystemClient;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services;

public class RestaurantService : IRestaurantService
{
    private readonly IBaseRepository<RestaurantDetail, Guid> _restaurantRepository;
    private readonly IBaseRepository<Image, Guid> _imageRepository;
    private readonly CloudinaryLogic _cloudinary;
    private readonly IBaseRepository<Partner, Guid> _partnerRepository;
    private readonly IPartnerService _partnerService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="restaurantRepository"></param>
    /// <param name="imageRepository"></param>
    /// <param name="cloudinary"></param>
    /// <param name="partnerRepository"></param>
    /// <param name="partnerService"></param>
    public RestaurantService(IBaseRepository<RestaurantDetail, Guid> restaurantRepository, IBaseRepository<Image, Guid> imageRepository, CloudinaryLogic cloudinary, IBaseRepository<Partner, Guid> partnerRepository, IPartnerService partnerService)
    {
        _restaurantRepository = restaurantRepository;
        _imageRepository = imageRepository;
        _cloudinary = cloudinary;
        _partnerRepository = partnerRepository;
        _partnerService = partnerService;
    }

    /// <summary>
    /// Select a specific restaurant by ID
    /// </summary>
    /// <param name="restaurantId"></param>
    /// <returns></returns>
    public async Task<Ecq220SelectRestaurantResponse> Ecq100SelectRestaurant(Guid restaurantId)
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
            .GetView<VwImage>(img => img.EntityId == restaurant.RestaurantId && img.EntityType == ConstantEnum.EntityType.Restaurant.ToString())
            .Select(img => img.ImageUrl)
            .ToListAsync())!;
        
        // True
        response.Response = restaurant;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        
        return response;
    }

    public async Task<Ecq220SelectRestaurantResponse> Ecq220SelectRestaurant(Guid restaurantId, IdentityEntity identityEntity)
    {
        var response = new Ecq220SelectRestaurantResponse { Success = false };
        
        // Get restaurant by ID with rating information
        var restaurant = await _restaurantRepository.GetView<VwRestaurant>(r => r.RestaurantId == restaurantId && r.PartnerId == Guid.Parse((identityEntity.UserId)))
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
            .GetView<VwImage>(img => img.EntityId == restaurant.RestaurantId && img.EntityType == ConstantEnum.EntityType.Restaurant.ToString())
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
    public async Task<Ecq220SelectRestaurantsResponse> Ecq100SelectRestaurants()
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
                .GetView<VwImage>(img => img.EntityId == restaurant.RestaurantId && img.EntityType == ConstantEnum.EntityType.Restaurant.ToString())
                .Select(img => img.ImageUrl)
                .ToListAsync())!;
        }
        
        // True
        response.Response = restaurants;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        
        return response;
    }

    public async Task<Ecq220SelectRestaurantsResponse> Ecq220SelectRestaurants(IdentityEntity identityEntity)
    {
        var response = new Ecq220SelectRestaurantsResponse { Success = false };
        
        // Select restaurants with rating information
        var restaurants = await _restaurantRepository.GetView<VwRestaurant>(x => x.PartnerId == Guid.Parse(identityEntity.UserId))
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
                .GetView<VwImage>(img => img.EntityId == restaurant.RestaurantId && img.EntityType == ConstantEnum.EntityType.Restaurant.ToString())
                .Select(img => img.ImageUrl)
                .ToListAsync())!;
        }
        
        // True
        response.Response = restaurants;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        
        return response;
    }
    
    /// <summary>
    /// Insert a new restaurant
    /// </summary>
    /// <param name="request">The request containing restaurant details</param>
    /// <param name="identityEntity">The identity of the user making the request</param>
    /// <returns>Response indicating success or failure</returns>
    public async Task<Ecq220InsertRestaurantResponse> InsertRestaurant(Ecq220InsertRestaurantRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq220InsertRestaurantResponse { Success = false };

        // Validate request - check if the partner has restaurant type
        var partnerExist = await _partnerRepository.Find(x => x.PartnerId == Guid.Parse(identityEntity.UserId)).FirstOrDefaultAsync();
        if (partnerExist == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.PartnerNotFound);
            return response;
        }

        // Begin transaction
        await _restaurantRepository.ExecuteInTransactionAsync(async () =>
        {
            // Insert new restaurant
            var newRestaurant = new RestaurantDetail
            {
                RestaurantId = Guid.NewGuid(),
                RestaurantName = request.RestaurantName,
                CuisineType = request.CuisineType,
                HasVegetarian = request.HasVegetarian,
                OpenTime = request.OpenTime,
                CloseTime = request.CloseTime,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                DestinationId = request.DestinationId,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                PartnerId = Guid.Parse(identityEntity.UserId),
            };
            
            await _restaurantRepository.AddAsync(newRestaurant);
            await _restaurantRepository.SaveChangesAsync(identityEntity.Email);

            // Insert restaurant images
            if (request.RestaurantImages.Any() && request.RestaurantImages.Count > 0)
            {
                foreach (var image in request.RestaurantImages)
                {
                    var imageUrl = await _cloudinary.UploadImageAsync(image);
                    var restaurantImage = new Image
                    {
                        EntityId = newRestaurant.RestaurantId,
                        ImageUrl = imageUrl,
                        EntityType = ConstantEnum.EntityType.Restaurant.ToString(),
                    };
                    await _imageRepository.AddAsync(restaurantImage);
                }
            }

            await _imageRepository.SaveChangesAsync(identityEntity.Email);

            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        
        return response;
    }

    /// <summary>
    /// Update an existing restaurant
    /// </summary>
    /// <param name="request">The request containing updated restaurant details</param>
    /// <param name="identityEntity">The identity of the user making the request</param>
    /// <returns>Response indicating success or failure</returns>
    public async Task<Ecq220UpdateRestaurantResponse> UpdateRestaurant(Ecq220UpdateRestaurantRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq220UpdateRestaurantResponse { Success = false };

        // Find the restaurant by ID
        var restaurant = await _restaurantRepository.Find(r => r.RestaurantId == request.RestaurantId && r.IsActive == true).FirstOrDefaultAsync();
        if (restaurant == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.RestaurantNotFound);
            return response;
        }
        
        // Verify that the restaurant belongs to the partner
        if (restaurant.PartnerId != Guid.Parse(identityEntity.UserId))
        {
            response.SetMessage(MessageId.I00000, CommonMessages.NotAuthorizedToManageRestaurant);
            return response;
        }

        // Begin transaction
        await _restaurantRepository.ExecuteInTransactionAsync(async () =>
        {
            // Update restaurant properties
            restaurant.RestaurantName = request.RestaurantName;
            restaurant.CuisineType = request.CuisineType;
            restaurant.HasVegetarian = request.HasVegetarian;
            restaurant.OpenTime = request.OpenTime;
            restaurant.CloseTime = request.CloseTime;
            restaurant.MinPrice = request.MinPrice;
            restaurant.MaxPrice = request.MaxPrice;
            restaurant.DestinationId = request.DestinationId;
            restaurant.PhoneNumber = request.PhoneNumber;
            restaurant.Address = request.Address;

            await _restaurantRepository.UpdateAsync(restaurant);
            await _restaurantRepository.SaveChangesAsync(identityEntity.Email);
            
            // Process images to remove
            if (request.ImagesToRemove != null && request.ImagesToRemove.Count > 0)
            {
                foreach (var imageUrl in request.ImagesToRemove)
                {
                    var imageToDelete = await _imageRepository.Find(img => 
                            img.EntityId == request.RestaurantId && 
                            img.EntityType == ConstantEnum.EntityType.Attraction.ToString() && 
                            img.ImageUrl == imageUrl)
                        .FirstOrDefaultAsync();

                    if (imageToDelete != null)
                    {
                        await _imageRepository.UpdateAsync(imageToDelete);
                    }
                }
                await _imageRepository.SaveChangesAsync(identityEntity.Email, true);
            }

            // Add new restaurant images if any
            if (request.RestaurantImages != null && request.RestaurantImages.Count > 0)
            {
                foreach (var image in request.RestaurantImages)
                {
                    var imageUrl = await _cloudinary.UploadImageAsync(image);
                    var restaurantImage = new Image
                    {
                        EntityId = restaurant.RestaurantId,
                        ImageUrl = imageUrl,
                        EntityType = ConstantEnum.EntityType.Restaurant.ToString(),
                    };
                    await _imageRepository.AddAsync(restaurantImage);
                }
                await _imageRepository.SaveChangesAsync(identityEntity.Email);
            }

            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });

        return response;
    }
}
