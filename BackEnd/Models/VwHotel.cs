using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class VwHotel
{
    public Guid HotelId { get; set; }

    public string HotelName { get; set; } = null!;

    public string? HotelDescription { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid? OwnerId { get; set; }

    public Guid? DestinationId { get; set; }

    public string? DestinationName { get; set; }

    public string? AddressLine { get; set; }

    public string? Ward { get; set; }

    public string? District { get; set; }

    public string? Province { get; set; }

    public decimal? AverageRating { get; set; }

    public int? TotalRatings { get; set; }
}
