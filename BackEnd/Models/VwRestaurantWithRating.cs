using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class VwRestaurantWithRating
{
    public Guid RestaurantId { get; set; }

    public Guid PartnerId { get; set; }

    public string? CuisineType { get; set; }

    public bool? HasVegetarian { get; set; }

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public Guid? DestinationId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public Guid? RatingId { get; set; }

    public Guid? UserId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? RatingCreatedAt { get; set; }

    public DateTime? RatingUpdatedAt { get; set; }

    public decimal? AverageRating { get; set; }

    public int? TotalRatings { get; set; }
}
