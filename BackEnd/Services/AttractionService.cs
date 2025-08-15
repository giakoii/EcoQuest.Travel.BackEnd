using BackEnd.DTOs.Ecq100;
using BackEnd.DTOs.Ecq230;
using BackEnd.Logics;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.SystemClient;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services;

public class AttractionService : IAttractionService
{
    private readonly IBaseRepository<AttractionDetail, Guid> _attractionRepository;
    private readonly IBaseRepository<Image, Guid> _imageRepository;
    private readonly CloudinaryLogic _cloudinary;
    private readonly IBaseRepository<Partner, Guid> _partnerRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attractionRepository"></param>
    /// <param name="imageRepository"></param>
    /// <param name="cloudinary"></param>
    /// <param name="partnerRepository"></param>
    public AttractionService(IBaseRepository<AttractionDetail, Guid> attractionRepository, IBaseRepository<Image, Guid> imageRepository, CloudinaryLogic cloudinary, IBaseRepository<Partner, Guid> partnerRepository)
    {
        _attractionRepository = attractionRepository;
        _imageRepository = imageRepository;
        _cloudinary = cloudinary;
        _partnerRepository = partnerRepository;
    }

    /// <summary>
    /// Select a specific attraction by ID
    /// </summary>
    /// <param name="attractionId"></param>
    /// <returns></returns>
    public async Task<Ecq230SelectAttractionResponse> Ecq100SelectAttraction(Guid attractionId)
    {
        var response = new Ecq230SelectAttractionResponse { Success = false };
        
        // Get attraction by ID with rating information
        var attraction = await _attractionRepository.GetView<VwAttraction>(a => a.AttractionId == attractionId)
            .Select(a => new Ecq230AttractionDetailEntity
            {
                AttractionId = a.AttractionId,
                OpenTime = StringUtil.ConvertToHhMm(a.OpenTime),
                CloseTime = StringUtil.ConvertToHhMm(a.CloseTime),
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(a.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(a.UpdatedAt),
                AttractionType = a.AttractionType,
                DestinationId = a.DestinationId,
                DestinationName = a.DestinationName,
                AverageRating = a.AverageRating,
                TotalRatings = a.TotalRatings,
                GuideAvailable = a.GuideAvailable,
                TicketPrice = a.TicketPrice,
                PhoneNumber = a.PhoneNumber,
                Address = a.Address,
                AttractionName = a.AttractionName!,
                AgeLimit = a.AgeLimit,
                PartnerId = a.PartnerId,
                DurationMinutes = a.DurationMinutes,
                UpdatedBy = a.UpdatedBy,
            })
            .FirstOrDefaultAsync();
        if (attraction == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.AttractionNotFound);
            return response;
        }
            
        // Fetch attraction images
        attraction.AttractionImages = (await _imageRepository
            .GetView<VwImage>(img => img.EntityId == attraction.AttractionId && img.EntityType == ConstantEnum.EntityType.Attraction.ToString())
            .Select(img => img.ImageUrl)
            .ToListAsync())!;
        
        // True
        response.Response = attraction;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        
        return response;
    }

    /// <summary>
    /// Select a attraction by ID for partner dashboard
    /// </summary>
    /// <param name="attractionId"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq230SelectAttractionResponse> Ecq230SelectAttraction(Guid attractionId, IdentityEntity identityEntity)
    {
         var response = new Ecq230SelectAttractionResponse { Success = false };
        
        // Get attraction by ID with rating information
        var attraction = await _attractionRepository.GetView<VwAttraction>(a => a.AttractionId == attractionId && a.PartnerId == Guid.Parse(identityEntity.UserId))
            .Select(a => new Ecq230AttractionDetailEntity
            {
                AttractionId = a.AttractionId,
                OpenTime = StringUtil.ConvertToHhMm(a.OpenTime),
                CloseTime = StringUtil.ConvertToHhMm(a.CloseTime),
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(a.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(a.UpdatedAt),
                AttractionType = a.AttractionType,
                DestinationId = a.DestinationId,
                DestinationName = a.DestinationName,
                AverageRating = a.AverageRating,
                TotalRatings = a.TotalRatings,
                GuideAvailable = a.GuideAvailable,
                TicketPrice = a.TicketPrice,
                PhoneNumber = a.PhoneNumber,
                Address = a.Address,
                AttractionName = a.AttractionName!,
                AgeLimit = a.AgeLimit,
                PartnerId = a.PartnerId,
                DurationMinutes = a.DurationMinutes,
                UpdatedBy = a.UpdatedBy,
            })
            .FirstOrDefaultAsync();
        if (attraction == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.AttractionNotFound);
            return response;
        }
            
        // Fetch attraction images
        attraction.AttractionImages = (await _imageRepository
            .GetView<VwImage>(img => img.EntityId == attraction.AttractionId && img.EntityType == ConstantEnum.EntityType.Attraction.ToString())
            .Select(img => img.ImageUrl)
            .ToListAsync())!;
        
        // True
        response.Response = attraction;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        
        return response;
    }

    /// <summary>
    /// Select all attractions
    /// </summary>
    /// <returns></returns>
    public async Task<Ecq100SelectAttractionsResponse> Ecq100SelectAttractions(Ecq100SelectAttractionsRequest request)
    {
        var response = new Ecq100SelectAttractionsResponse { Success = false };
        
        // Select attractions with rating information
        var attractions = await _attractionRepository.GetView<VwAttraction>()
            .Select(a => new Ecq100SelectAttractionsEntity
            {
                AttractionId = a.AttractionId,
                EntryFee = a.TicketPrice,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(a.CreatedAt),
                AttractionType = a.AttractionType,
                DestinationId = a.DestinationId,
                DestinationName = a.DestinationName,
                AverageRating = a.AverageRating,
                TotalRatings = a.TotalRatings,
                AttractionName = a.AttractionName!,
            })
            .ToListAsync();

        if (request.DestinationIds != null)
        {
            attractions = attractions
                .Where(h => h.DestinationId.HasValue && request.DestinationIds.Contains(h.DestinationId.Value))
                .ToList();
        }
        
        // Fetch images for each attraction
        foreach (var attraction in attractions)
        {
            attraction.AttractionImages = (await _imageRepository
                .GetView<VwImage>(img => img.EntityId == attraction.AttractionId && img.EntityType == ConstantEnum.EntityType.Attraction.ToString())
                .Select(img => img.ImageUrl)
                .ToListAsync())!;
        }
        
        // True
        response.Response = attractions;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        
        return response;
    }

    /// <summary>
    /// Select attractions for partner dashboard
    /// </summary>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq230SelectAttractionsResponse> Ecq230SelectAttractions(IdentityEntity identityEntity)
    {
        var response = new Ecq230SelectAttractionsResponse { Success = false };
        
        // Select attractions with rating information
        var attractions = await _attractionRepository.GetView<VwAttraction>(a => a.PartnerId == Guid.Parse(identityEntity.UserId))
            .Select(a => new Ecq230AttractionEntity
            {
                AttractionId = a.AttractionId,
                EntryFee = a.TicketPrice,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(a.CreatedAt),
                AttractionType = a.AttractionType,
                DestinationId = a.DestinationId,
                DestinationName = a.DestinationName,
                AverageRating = a.AverageRating,
                TotalRatings = a.TotalRatings,
                AttractionName = a.AttractionName!,
            })
            .ToListAsync();
        
        // Fetch images for each attraction
        foreach (var attraction in attractions)
        {
            attraction.AttractionImages = (await _imageRepository
                .GetView<VwImage>(img => img.EntityId == attraction.AttractionId && img.EntityType == ConstantEnum.EntityType.Attraction.ToString())
                .Select(img => img.ImageUrl)
                .ToListAsync())!;
        }
        
        // True
        response.Response = attractions;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        
        return response;
    }
    
    /// <summary>
    /// Insert a new attraction
    /// </summary>
    /// <param name="request">The request containing attraction details</param>
    /// <param name="identityEntity">The identity of the user making the request</param>
    /// <returns>Response indicating success or failure</returns>
    public async Task<Ecq230InsertAttractionResponse> InsertAttraction(Ecq230InsertAttractionRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq230InsertAttractionResponse { Success = false };

        // Validate request - check if the partner has attraction type
        var partnerExist = await _partnerRepository.Find(x => x.PartnerId == Guid.Parse(identityEntity.UserId) && x.IsActive == true).FirstOrDefaultAsync();
        if (partnerExist == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.PartnerNotFound);
            return response;
        }

        // Begin transaction
        await _attractionRepository.ExecuteInTransactionAsync(async () =>
        {
            // Insert new attraction
            var newAttraction = new AttractionDetail
            {
                AttractionId = Guid.NewGuid(),
                AttractionName = request.AttractionName,
                AttractionType = request.AttractionType,
                TicketPrice = request.TicketPrice,
                OpenTime = request.OpenTime,
                CloseTime = request.CloseTime,
                DestinationId = request.DestinationId,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                GuideAvailable = request.GuideAvailable,
                AgeLimit = request.AgeLimit,
                DurationMinutes = request.DurationMinutes,
                PartnerId = Guid.Parse(identityEntity.UserId),
            };
            
            await _attractionRepository.AddAsync(newAttraction);
            await _attractionRepository.SaveChangesAsync(identityEntity.Email);

            // Insert attraction images
            if (request.AttractionImages.Any() && request.AttractionImages.Count > 0)
            {
                foreach (var image in request.AttractionImages)
                {
                    var imageUrl = await _cloudinary.UploadImageAsync(image);
                    var attractionImage = new Image
                    {
                        EntityId = newAttraction.AttractionId,
                        ImageUrl = imageUrl,
                        EntityType = ConstantEnum.EntityType.Attraction.ToString(),
                    };
                    await _imageRepository.AddAsync(attractionImage);
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
    /// Update an existing attraction
    /// </summary>
    /// <param name="request">The request containing updated attraction details</param>
    /// <param name="identityEntity">The identity of the user making the request</param>
    /// <returns>Response indicating success or failure</returns>
    public async Task<Ecq230UpdateAttractionResponse> Ecq230UpdateAttraction(Ecq230UpdateAttractionRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq230UpdateAttractionResponse { Success = false };

        // Find the attraction by ID
        var attraction = await _attractionRepository.Find(a => a.AttractionId == request.AttractionId && a.IsActive == true).FirstOrDefaultAsync();
        if (attraction == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.AttractionNotFound);
            return response;
        }
        
        // Verify that the attraction belongs to the partner
        if (attraction.PartnerId != Guid.Parse(identityEntity.UserId))
        {
            response.SetMessage(MessageId.I00000, CommonMessages.NotAuthorizedToAttraction);
            return response;
        }

        // Begin transaction
        await _attractionRepository.ExecuteInTransactionAsync(async () =>
        {
            // Update attraction properties
            attraction.AttractionName = request.AttractionName;
            attraction.AttractionType = request.AttractionType;
            attraction.TicketPrice = request.TicketPrice;
            attraction.OpenTime = request.OpenTime;
            attraction.CloseTime = request.CloseTime;
            attraction.DestinationId = request.DestinationId;
            attraction.PhoneNumber = request.PhoneNumber;
            attraction.Address = request.Address;
            attraction.GuideAvailable = request.GuideAvailable;
            attraction.AgeLimit = request.AgeLimit;
            attraction.DurationMinutes = request.DurationMinutes;

            await _attractionRepository.UpdateAsync(attraction);
            await _attractionRepository.SaveChangesAsync(identityEntity.Email);

            // Process images to remove
            if (request.ImagesToRemove != null && request.ImagesToRemove.Count > 0)
            {
                foreach (var imageUrl in request.ImagesToRemove)
                {
                    var imageToDelete = await _imageRepository.Find(img => 
                            img.EntityId == request.AttractionId && 
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

            // Add new attraction images if any
            if (request.AttractionImages != null && request.AttractionImages.Count > 0)
            {
                foreach (var image in request.AttractionImages)
                {
                    var imageUrl = await _cloudinary.UploadImageAsync(image);
                    var attractionImage = new Image
                    {
                        EntityId = attraction.AttractionId,
                        ImageUrl = imageUrl,
                        EntityType = ConstantEnum.EntityType.Attraction.ToString(),
                    };
                    await _imageRepository.AddAsync(attractionImage);
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
