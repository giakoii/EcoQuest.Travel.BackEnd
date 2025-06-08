using BackEnd.DTOs.Ecq210;
using BackEnd.Logics;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.SystemClient;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services;

public class HotelService : IHotelService
{
    private readonly IBaseRepository<Hotel, Guid> _hotelRepository;
    private readonly IPartnerService _partnerService;
    private readonly IBaseRepository<Image, Guid> _imageRepository;
    private readonly CloudinaryLogic _cloudinary;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hotelRepository"></param>
    /// <param name="partnerService"></param>
    /// <param name="cloudinary"></param>
    /// <param name="imageRepository"></param>
    public HotelService(IBaseRepository<Hotel, Guid> hotelRepository, IPartnerService partnerService,
        CloudinaryLogic cloudinary, IBaseRepository<Image, Guid> imageRepository)
    {
        _hotelRepository = hotelRepository;
        _partnerService = partnerService;
        _cloudinary = cloudinary;
        _imageRepository = imageRepository;
    }

    /// <summary>
    /// Insert new hotel
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq210InsertHotelResponse> InsertHotel(Ecq210InsertHotelRequest request,
        IdentityEntity identityEntity)
    {
        var response = new Ecq210InsertHotelResponse { Success = false };

        // Validate request
        var partnerTypeValid = await _partnerService.SelectPartner(Guid.Parse(identityEntity.UserId));
        if (!partnerTypeValid.Response.PartnerType.Contains((byte)ConstantEnum.PartnerType.Hotel))
        {
            response.SetMessage(MessageId.I00000, CommonMessages.PartnerTypeInvalid);
            return response;
        }

        // Begin transaction
        await _hotelRepository.ExecuteInTransactionAsync(async () =>
        {
            // Insert new hotel
            var newHotel = new Hotel
            {
                Name = request.Name,
                Address = request.Address,
                Description = request.Description,
                OwnerId = Guid.Parse(identityEntity.UserId),
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                DestinationId = request.DestinationId,
            };
            await _hotelRepository.AddAsync(newHotel);
            await _hotelRepository.SaveChangesAsync(identityEntity.Email);

            // Insert hotel images
            if (request.HotelImages != null && request.HotelImages.Count > 0)
            {
                foreach (var image in request.HotelImages)
                {
                    var imageUrl = await _cloudinary.UploadImageAsync(image);
                    var hotelImage = new Image
                    {
                        EntityId = newHotel.HotelId,
                        ImageUrl = imageUrl,
                        EntityType = ConstantEnum.EntityImage.Hotel.ToString(),
                    };
                    await _imageRepository.AddAsync(hotelImage);
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
    /// Select hotel by request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Ecq210SelectHotelResponse> SelectHotel(Ecq210SelectHotelRequest request)
    {
        var response = new Ecq210SelectHotelResponse { Success = false };

        // Select hotel by request
        var hotelSelect = await _hotelRepository
            .GetView<VwHotel>()
            .Where(h => h.HotelId == request.HotelId)
            .Select(h => new Ecq210SelectHotelEntity
            {
                HotelId = h.HotelId,
                HotelName = h.HotelName,
                HotelDescription = h.HotelDescription,
                Address = h.Address,
                PhoneNumber = h.PhoneNumber,
                Email = h.Email,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(h.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(h.UpdatedAt),
                OwnerId = h.OwnerId,
                DestinationId = h.DestinationId,
                DestinationName = h.DestinationName,
                AverageRating = h.AverageRating,
                TotalRatings = h.TotalRatings,
            })
            .FirstOrDefaultAsync();

        // Select hotel rooms
        hotelSelect!.Rooms = await _hotelRepository
            .GetView<VwHotelRoom>()
            .Where(r => r.HotelId == hotelSelect.HotelId)
            .Select(r => new Ecq310SelectPartnerEntityHotelRoom
            {
                RoomId = r.RoomId,
                Description = r.Description,
                HotelId = r.HotelId,
                IsAvailable = r.IsAvailable,
                MaxGuests = r.MaxGuests,
                RoomType = r.RoomType,
                PricePerNight = r.PricePerNight,
            })
            .ToListAsync();
        
        // Select hotel images
        var hotelImageUrls = await _imageRepository
            .GetView<VwImage>( x => x.EntityId == hotelSelect.HotelId && x.EntityType == ConstantEnum.EntityImage.Hotel.ToString())
            .Select(x => x.ImageUrl)
            .ToListAsync();
        
        hotelSelect.HotelImages = hotelImageUrls!;

        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        response.Response = hotelSelect;
        return response;
    }

    /// <summary>
    /// Select a list of all hotels with their basic information and images
    /// </summary>
    /// <returns></returns>
    public async Task<Ecq210SelectHotelsResponse> SelectHotels()
    {
        var response = new Ecq210SelectHotelsResponse { Success = false };
        
        // Select hotels
        var hotels = await _hotelRepository.GetView<VwHotel>()
            .Select(h => new Ecq210HotelEntity
            {
                HotelId = h.HotelId,
                Name = h.HotelName,
                Description = h.HotelDescription,
                AddressLine = h.AddressLine,
                Ward = h.Ward,
                District = h.District,
                Province = h.Province,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(h.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(h.UpdatedAt),
                OwnerId = h.OwnerId,
                DestinationId = h.DestinationId,
                DestinationName = h.DestinationName,
                AverageRating = h.AverageRating,
                TotalRatings = h.TotalRatings,
            })
            .ToListAsync();
        
        // Fetch images for each hotel
        foreach (var hotel in hotels)
        {
            hotel.HotelImages = (await _imageRepository
                .GetView<VwImage>(img => img.EntityId == hotel.HotelId && img.EntityType == ConstantEnum.EntityImage.Hotel.ToString())
                .Select(img => img.ImageUrl)
                .ToListAsync())!;
        }
        
        // True
        response.Response = hotels;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }
}

