using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq111;

public class Ecq111SelectHotelRoomResponse : AbstractApiResponse<Ecq111HotelRoomDetailEntity>
{
    public override Ecq111HotelRoomDetailEntity Response { get; set; }
}

public class Ecq111HotelRoomDetailEntity
{
    public Guid RoomId { get; set; }
    
    public Guid HotelId { get; set; }
    
    public string HotelName { get; set; } = null!;
    
    public string RoomType { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public int MaxGuests { get; set; }
    
    public decimal PricePerNight { get; set; }
    
    public bool IsAvailable { get; set; }
    
    public string CreatedAt { get; set; }
    
    public string UpdatedAt { get; set; }
    
    public string HotelAddress { get; set; } = null!;
    
    public string HotelEmail { get; set; } = null!;
    
    public string HotelPhoneNumber { get; set; } = null!;
    
    public string HotelDescription { get; set; } = null!;
    
    public string Ward { get; set; } = null!;
    
    public string District { get; set; } = null!;
    
    public string Province { get; set; } = null!;
    
    public List<string>? RoomImages { get; set; }
}
