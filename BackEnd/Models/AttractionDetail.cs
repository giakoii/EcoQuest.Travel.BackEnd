using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class AttractionDetail
{
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

    public virtual ICollection<AttractionRating> AttractionRatings { get; set; } = new List<AttractionRating>();

    public virtual Destination? Destination { get; set; }

    public virtual Partner Partner { get; set; } = null!;
}
