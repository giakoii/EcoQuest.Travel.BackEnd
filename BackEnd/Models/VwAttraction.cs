using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class VwAttraction
{
    public Guid AttractionId { get; set; }

    public string? AttractionName { get; set; }

    public Guid PartnerId { get; set; }

    public string? AttractionType { get; set; }

    public decimal? TicketPrice { get; set; }

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public Guid? DestinationId { get; set; }

    public string? DestinationName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public bool? GuideAvailable { get; set; }

    public int? AgeLimit { get; set; }

    public int? DurationMinutes { get; set; }

    public decimal? AverageRating { get; set; }

    public int? TotalRatings { get; set; }
}
