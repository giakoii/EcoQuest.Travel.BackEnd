using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq210;

public class Ecq210SelectHotelsResponse : AbstractApiResponse<List<Ecq210HotelEntity>>
{
    public override List<Ecq210HotelEntity> Response { get; set; }
}

public class Ecq210HotelEntity
{
    public Guid HotelId { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public decimal MinPrice { get; set; }
    
    public decimal MaxPrice { get; set; }
    
    public string? AddressLine { get; set; }
    
    public string? Ward { get; set; }
    
    public string? District { get; set; }
    
    public string? Province { get; set; }
    
    public string CreatedAt { get; set; }
    
    public string UpdatedAt { get; set; }
    
    public Guid? OwnerId { get; set; }
    
    public Guid? DestinationId { get; set; }
    
    public string? DestinationName { get; set; }
    
    public decimal? AverageRating { get; set; }
    
    public int? TotalRatings { get; set; }
    
    public List<string>? HotelImages { get; set; }
}
