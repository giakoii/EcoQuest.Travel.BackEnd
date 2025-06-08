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

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attractionRepository"></param>
    /// <param name="imageRepository"></param>
    /// <param name="cloudinary"></param>
    public AttractionService(IBaseRepository<AttractionDetail, Guid> attractionRepository, 
                             IBaseRepository<Image, Guid> imageRepository,
                             CloudinaryLogic cloudinary)
    {
        _attractionRepository = attractionRepository;
        _imageRepository = imageRepository;
        _cloudinary = cloudinary;
    }

    /// <summary>
    /// Select a specific attraction by ID
    /// </summary>
    /// <param name="attractionId"></param>
    /// <returns></returns>
    public async Task<Ecq230SelectAttractionResponse> SelectAttraction(Guid attractionId)
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
            .GetView<VwImage>(img => img.EntityId == attraction.AttractionId && img.EntityType == ConstantEnum.EntityImage.Attraction.ToString())
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
    public async Task<Ecq230SelectAttractionsResponse> SelectAttractions()
    {
        var response = new Ecq230SelectAttractionsResponse { Success = false };
        
        // Select attractions with rating information
        var attractions = await _attractionRepository.GetView<VwAttraction>()
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
                .GetView<VwImage>(img => img.EntityId == attraction.AttractionId && img.EntityType == ConstantEnum.EntityImage.Attraction.ToString())
                .Select(img => img.ImageUrl)
                .ToListAsync())!;
        }
        
        // True
        response.Response = attractions;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        
        return response;
    }
}
