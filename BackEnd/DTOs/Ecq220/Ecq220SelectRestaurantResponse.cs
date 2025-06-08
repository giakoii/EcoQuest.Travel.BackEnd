using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq220;

public class Ecq220SelectRestaurantResponse : AbstractApiResponse<Ecq220RestaurantDetailEntity>
{
    public override Ecq220RestaurantDetailEntity Response { get; set; }
}

public class Ecq220RestaurantDetailEntity
{
    public Guid RestaurantId { get; set; }

    public string? RestaurantName { get; set; }

    public Guid PartnerId { get; set; }

    public string? CuisineType { get; set; }

    public bool? HasVegetarian { get; set; }

    public string OpenTime { get; set; }

    public string CloseTime { get; set; }

    public string CreatedAt { get; set; }

    public string UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public Guid? DestinationId { get; set; }

    public string? DestinationName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }
    
    public decimal? AverageRating { get; set; }
    
    public int? TotalRatings { get; set; }
    
    public List<string>? RestaurantImages { get; set; }
}
