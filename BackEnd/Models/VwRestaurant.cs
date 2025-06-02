using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class VwRestaurant
{
    public Guid PartnerId { get; set; }

    public string? CuisineType { get; set; }

    public bool? HasVegetarian { get; set; }

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public Guid? DestinationId { get; set; }

    public string? DestinationName { get; set; }

    public string? AddressLine { get; set; }

    public string? Ward { get; set; }
    
    public string? District { get; set; }

    public string? Province { get; set; }
}
