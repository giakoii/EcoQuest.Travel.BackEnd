﻿using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class VwRestaurant
{
    public Guid RestaurantId { get; set; }

    public string? RestaurantName { get; set; }

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

    public string? DestinationName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public decimal? AverageRating { get; set; }

    public int? TotalRatings { get; set; }
}
