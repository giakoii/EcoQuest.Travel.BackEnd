using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class RestaurantDetail
{
    public Guid PartnerId { get; set; }

    public string? CuisineType { get; set; }

    public bool? HasVegetarian { get; set; }

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsActive { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public Guid? DestinationId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public virtual Destination? Destination { get; set; }

    public virtual Partner Partner { get; set; } = null!;

    public virtual ICollection<RestaurantRating> RestaurantRatings { get; set; } = new List<RestaurantRating>();
}
