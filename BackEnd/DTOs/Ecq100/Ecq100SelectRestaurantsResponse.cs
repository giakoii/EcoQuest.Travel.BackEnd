using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100SelectRestaurantsResponse : AbstractApiResponse<List<Ecq100SelectRestaurantsEntity>>
{
    public override List<Ecq100SelectRestaurantsEntity> Response { get; set; }
}

public class Ecq100SelectRestaurantsEntity
{
    public Guid RestaurantId { get; set; }
    
    public string RestaurantName { get; set; } = null!;
    
    public string? AddressLine { get; set; }
    public string? OpenTime { get; set; }
    
    public string? CloseTime { get; set; }
    
    public string CreatedAt { get; set; }
    
    public string UpdatedAt { get; set; }
    
    public decimal? MinPrice { get; set; }
    
    public decimal? MaxPrice { get; set; }
    
    public Guid? DestinationId { get; set; }
    
    public string? DestinationName { get; set; }
    
    public List<string>? RestaurantImages { get; set; }
    
    // Rating information
    public decimal? AverageRating { get; set; }
    
    public int? TotalRatings { get; set; }

}