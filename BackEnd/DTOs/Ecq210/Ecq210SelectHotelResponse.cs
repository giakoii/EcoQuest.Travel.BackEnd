using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq210;

public class Ecq210SelectHotelResponse : AbstractApiResponse<Ecq210SelectHotelEntity>
{
    public override Ecq210SelectHotelEntity Response { get; set; }
}

public class Ecq210SelectHotelEntity
{public Guid HotelId { get; set; }

    public string HotelName { get; set; } = null!;

    public string? HotelDescription { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string CreatedAt { get; set; }

    public string UpdatedAt { get; set; }

    public Guid? OwnerId { get; set; }

    public Guid? DestinationId { get; set; }

    public string? DestinationName { get; set; }
    
    public decimal? AverageRating { get; set; }
    
    public int? TotalRatings { get; set; }
    
    public List<string>? HotelImages { get; set; }
    
    public List<Ecq310SelectPartnerEntityHotelRoom> Rooms { get; set; }
}

public class Ecq310SelectPartnerEntityHotelRoom
{
    public Guid RoomId { get; set; }

    public Guid HotelId { get; set; }

    public string RoomType { get; set; } = null!;

    public decimal PricePerNight { get; set; }

    public int MaxGuests { get; set; }

    public string? Description { get; set; }

    public bool? IsAvailable { get; set; }
}