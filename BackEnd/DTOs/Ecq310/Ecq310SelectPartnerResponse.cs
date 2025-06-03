using BackEnd.Controllers;
using BackEnd.Models;

namespace BackEnd.DTOs.Ecq310;

public class Ecq310SelectPartnerResponse : AbstractApiResponse<Ecq310SelectPartnerEntity>
{
    public override Ecq310SelectPartnerEntity Response { get; set; }
}

public class Ecq310SelectPartnerEntity
{
    public Guid PartnerId { get; set; }

    public Guid AccountId { get; set; }

    public string? CompanyName { get; set; }

    public string ContactName { get; set; }

    public string? Phone { get; set; }

    public string? Description { get; set; }

    public bool? Verified { get; set; }
    
    public string CreatedAt { get; set; }
    
    public List<byte> PartnerType { get; set; }
    
    public List<Ecq310SelectPartnerEntityHotel> Hotels { get; set; } 
    
    public List<Ecq310SelectPartnerEntityRestaurant> Restaurants { get; set; }
    
    public List<Ecq310SelectPartnerEntityAttractionDetail> AttractionDetails { get; set; }
}

public class Ecq310SelectPartnerEntityAttractionDetail
{
    public Guid PartnerId { get; set; }

    public string? AttractionType { get; set; }

    public decimal? TicketPrice { get; set; }

    public string OpenTime { get; set; }

    public string CloseTime { get; set; }

    public string CreatedAt { get; set; }

    public string UpdatedAt { get; set; }

    public Guid? DestinationId { get; set; }

    public string? DestinationName { get; set; }

    public string? AddressLine { get; set; }

    public string? Ward { get; set; }
    
    public string? District { get; set; }

    public string? Province { get; set; }
}

public class Ecq310SelectPartnerEntityRestaurant
{
    public Guid PartnerId { get; set; }

    public string? CuisineType { get; set; }

    public bool? HasVegetarian { get; set; }

    public string OpenTime { get; set; }

    public string CloseTime { get; set; }

    public string CreatedAt { get; set; }

    public string UpdatedAt { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public Guid? DestinationId { get; set; }

    public string? DestinationName { get; set; }

    public string? AddressLine { get; set; }

    public string? Ward { get; set; }
    
    public string? District { get; set; }

    public string? Province { get; set; }
}

public class Ecq310SelectPartnerEntityHotel
{
    public Guid HotelId { get; set; }

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

    public string? AddressLine { get; set; }

    public string? Ward { get; set; }
    
    public string? District { get; set; }

    public string? Province { get; set; }
    
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