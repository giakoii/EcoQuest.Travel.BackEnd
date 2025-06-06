using BackEnd.DTOs.Ecq211;
using BackEnd.Logics;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.SystemClient;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services;

public class HotelRoomService : IHotelRoomService
{
    private readonly IBaseRepository<HotelRoom, Guid> _hotelRoomRepository;
    private readonly IBaseRepository<Hotel, Guid> _hotelRepository;
    private readonly IBaseRepository<Image, Guid> _imageRepository;
    private readonly CloudinaryLogic _cloudinary;
    
    public HotelRoomService(IBaseRepository<HotelRoom, Guid> hotelRoomRepository, IBaseRepository<Hotel, Guid> hotelRepository, CloudinaryLogic cloudinary, IBaseRepository<Image, Guid> imageRepository)
    {
        _hotelRoomRepository = hotelRoomRepository;
        _hotelRepository = hotelRepository;
        _cloudinary = cloudinary;
        _imageRepository = imageRepository;
    }
    
    /// <summary>
    /// Insert a new hotel room
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq211InsertHotelRoomResponse> InsertHotelRoom(Ecq211InsertHotelRoomRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq211InsertHotelRoomResponse { Success = false };
        
        // Verify hotel exists and user is the hotel owner
        var hotel = await _hotelRepository.Find(h => h.HotelId == request.HotelId && h.IsActive == true).FirstOrDefaultAsync();
        if (hotel == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.HotelNotFound);
            return response;
        }
        
        // Check if user is the owner of the hotel
        if (hotel.OwnerId != Guid.Parse(identityEntity.UserId))
        {
            response.SetMessage(MessageId.I00000, CommonMessages.NotAuthorizedToManageHotel);
            return response;
        }
        
        // Begin transaction
        await _hotelRoomRepository.ExecuteInTransactionAsync(async () =>
        {
            // Insert new room
            var newHotelRoom = new HotelRoom
            {
                HotelId = request.HotelId,
                RoomType = request.RoomType,
                Description = request.Description,
                MaxGuests = request.MaxGuests,
                PricePerNight = request.PricePerNight,
                IsAvailable = true,
                Area = request.Area,
                BedType = Enum.GetName(typeof(ConstantEnum.BedType), request.BedType),
                NumberOfBeds = request.NumberOfBeds,
                NumberOfRoomsAvailable = request.NumberOfRoomsAvailable,
                HasPrivateBathroom = request.HasPrivateBathroom,
                HasAirConditioner = request.HasAirConditioner,
                HasWifi = request.HasWifi,
                HasBreakfast = request.HasBreakfast,
                HasTv = request.HasTv,
                HasMinibar = request.HasMinibar,
                HasBalcony = request.HasBalcony,
                HasWindow = request.HasWindow,
                IsRefundable = request.IsRefundable,
                FreeCancellationUntil = request.FreeCancellationUntil,
                SmokingAllowed = request.SmokingAllowed,
                CheckinTime = request.CheckinTime,
                CheckoutTime = request.CheckoutTime,
                SpecialNote = request.SpecialNote
            };
            
            // Save to database
            await _hotelRoomRepository.AddAsync(newHotelRoom);
            await _hotelRoomRepository.SaveChangesAsync(identityEntity.Email);
            
            // Insert hotel room images
            if (request.HotelRoomImages != null && request.HotelRoomImages.Count > 0)
            {
                foreach (var image in request.HotelRoomImages)
                {
                    var imageUrl = await _cloudinary.UploadImageAsync(image);
                    var hotelImage = new Image
                    {
                        EntityId = newHotelRoom.RoomId,
                        ImageUrl = imageUrl,
                        EntityType = ConstantEnum.EntityImage.HotelRoom.ToString(),
                    };
                    await _imageRepository.AddAsync(hotelImage);
                }
            }
            await _imageRepository.SaveChangesAsync(identityEntity.Email);
            
            // True
            response.Response = newHotelRoom.RoomId.ToString();
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        
        return response;
    }
    
    /// <summary>
    /// Select all rooms for a specific hotel
    /// </summary>
    /// <param name="hotelId"></param>
    /// <returns></returns>
    public async Task<Ecq211SelectHotelRoomsResponse> SelectHotelRooms(Guid hotelId)
    {
        var response = new Ecq211SelectHotelRoomsResponse { Success = false };
        
        // Verify hotel exists
        var hotel = await _hotelRepository.Find(h => h.HotelId == hotelId && h.IsActive == true).FirstOrDefaultAsync();
        if (hotel == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.HotelNotFound);
            return response;
        }
        
        // Get all rooms for the hotel
        var rooms = await _hotelRoomRepository.GetView<VwHotelRoom>(r => r.HotelId == hotelId)
            .Select(r => new Ecq211HotelRoomEntity
            {
                RoomId = r.RoomId,
                HotelId = r.HotelId,
                RoomType = r.RoomType,
                Description = r.Description!,
                MaxGuests = r.MaxGuests,
                PricePerNight = r.PricePerNight,
                IsAvailable = r.IsAvailable,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(r.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(r.UpdatedAt)
            })
            .ToListAsync();
        
        // Fetch images for each room
        foreach (var room in rooms)
        {
            room.HotelRoomImages = await _imageRepository
                .GetView<VwImage>(x => x.EntityId == room.RoomId && x.EntityType == ConstantEnum.EntityImage.HotelRoom.ToString())
                .Select(x => x.ImageUrl)
                .ToListAsync();
        }
        
        // True
        response.Response = rooms;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        
        return response;
    }
    
    /// <summary>
    /// Select a specific hotel room by ID
    /// </summary>
    /// <param name="roomId"></param>
    /// <returns></returns>
    public async Task<Ecq211SelectHotelRoomResponse> SelectHotelRoom(Guid roomId)
    {
        var response = new Ecq211SelectHotelRoomResponse { Success = false };
        
        // Get the room
        var room = await _hotelRoomRepository.GetView<VwHotelRoom>(r => r.RoomId == roomId)
            .Select(r => new Ecq211HotelRoomDetailEntity
            {
                RoomId = r.RoomId,
                HotelId = r.HotelId,
                RoomType = r.RoomType,
                Description = r.Description!,
                MaxGuests = r.MaxGuests,
                PricePerNight = r.PricePerNight,
                IsAvailable = r.IsAvailable,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(r.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(r.UpdatedAt),
                Area = r.Area,
                BedType = r.BedType,
                NumberOfBeds = r.NumberOfBeds,
                NumberOfRoomsAvailable = r.NumberOfRoomsAvailable,
                HasPrivateBathroom = r.HasPrivateBathroom,
                HasAirConditioner = r.HasAirConditioner,
                HasWifi = r.HasWifi,
                HasBreakfast = r.HasBreakfast,
                HasTv = r.HasTv,
                HasMinibar = r.HasMinibar,
                HasBalcony = r.HasBalcony,
                HasWindow = r.HasWindow,
                IsRefundable = r.IsRefundable,
                FreeCancellationUntil = r.FreeCancellationUntil,
                SmokingAllowed = r.SmokingAllowed,
                CheckinTime = r.CheckinTime,
                CheckoutTime = r.CheckoutTime,
                SpecialNote = r.SpecialNote
            })
            .FirstOrDefaultAsync();
        if (room == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.HotelRoomNotFound);
            return response;
        }
        
        // Fetch room images
        room.HotelRoomImages = await _imageRepository
            .GetView<VwImage>(x => x.EntityId == room.RoomId && x.EntityType == ConstantEnum.EntityImage.HotelRoom.ToString())
            .Select(x => x.ImageUrl)
            .ToListAsync();
        
        // True
        response.Response = room;
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        
        return response;
    }
    
    /// <summary>
    /// Update an existing hotel room
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq211UpdateHotelRoomResponse> UpdateHotelRoom(Ecq211UpdateHotelRoomRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq211UpdateHotelRoomResponse { Success = false };
        
        // Get the room
        var room = await _hotelRoomRepository.Find(r => r.RoomId == request.RoomId && r.IsActive == true).FirstOrDefaultAsync();
        if (room == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.HotelRoomNotFound);
            return response;
        }
        
        // Check if user is the owner of the hotel
        if (room.Hotel.OwnerId != Guid.Parse(identityEntity.UserId))
        {
            response.SetMessage(MessageId.I00000, CommonMessages.NotAuthorizedToManageHotel);
            return response;
        }
        
        // Update room properties
        room.RoomType = request.RoomType;
        room.Description = request.Description;
        room.MaxGuests = request.MaxGuests;
        room.PricePerNight = request.PricePerNight;
        room.IsAvailable = request.IsAvailable;
        
        // Save changes
        _hotelRoomRepository.Update(room);
        await _hotelRoomRepository.SaveChangesAsync(identityEntity.Email);
        
        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        
        return response;
    }
}
