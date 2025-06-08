using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class VwAttractionWithRating
{
    public Guid AttractionId { get; set; }

    public Guid PartnerId { get; set; }

    public string? AttractionType { get; set; }

    public decimal? TicketPrice { get; set; }

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsActive { get; set; }

    public Guid? DestinationId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public bool? GuideAvailable { get; set; }

    public int? AgeLimit { get; set; }

    public int? DurationMinutes { get; set; }

    public Guid? RatingId { get; set; }

    public Guid? UserId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? RatingCreatedAt { get; set; }

    public DateTime? RatingUpdatedAt { get; set; }

    public string? RatingCreatedBy { get; set; }

    public string? RatingUpdatedBy { get; set; }

    public decimal? AverageRating { get; set; }

    public int? TotalRatings { get; set; }
}
