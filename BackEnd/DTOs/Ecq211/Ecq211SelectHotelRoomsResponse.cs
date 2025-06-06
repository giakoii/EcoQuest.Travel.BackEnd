using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq211;

public class Ecq211SelectHotelRoomsResponse : AbstractApiResponse<List<Ecq211HotelRoomEntity>>
{
    public override List<Ecq211HotelRoomEntity> Response { get; set; }
}

public class Ecq211HotelRoomEntity
{
    public Guid RoomId { get; set; }
    
    public Guid HotelId { get; set; }
    
    public string RoomType { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public int MaxGuests { get; set; }
    
    public decimal PricePerNight { get; set; }
    
    public bool? IsAvailable { get; set; }
    
    public string CreatedAt { get; set; }
    
    public string UpdatedAt { get; set; }
    
    public List<string?>? HotelRoomImages { get; set; }
}
