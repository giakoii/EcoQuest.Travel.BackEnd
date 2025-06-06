using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq211;

public class Ecq211SelectHotelRoomResponse : AbstractApiResponse<Ecq211HotelRoomDetailEntity>
{
    public override Ecq211HotelRoomDetailEntity Response { get; set; }
}

public class Ecq211HotelRoomDetailEntity
{
    public Guid RoomId { get; set; }
    
    public Guid HotelId { get; set; }
    
    public string HotelName { get; set; } = null!;
    
    public string RoomType { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public int MaxGuests { get; set; }
    
    public decimal PricePerNight { get; set; }
    
    public bool? IsAvailable { get; set; }
    
    public string CreatedAt { get; set; }
    
    public string UpdatedAt { get; set; }
    
    public int? Area { get; set; }

    public string? BedType { get; set; }

    public int? NumberOfBeds { get; set; }

    public int? NumberOfRoomsAvailable { get; set; }

    public bool? HasPrivateBathroom { get; set; }

    public bool? HasAirConditioner { get; set; }

    public bool? HasWifi { get; set; }

    public bool? HasBreakfast { get; set; }

    public bool? HasTv { get; set; }

    public bool? HasMinibar { get; set; }

    public bool? HasBalcony { get; set; }

    public bool? HasWindow { get; set; }

    public bool? IsRefundable { get; set; }

    public DateTime? FreeCancellationUntil { get; set; }

    public bool? SmokingAllowed { get; set; }

    public TimeOnly? CheckinTime { get; set; }

    public TimeOnly? CheckoutTime { get; set; }

    public string? SpecialNote { get; set; }
    
    public List<string?> HotelRoomImages { get; set; }
}
