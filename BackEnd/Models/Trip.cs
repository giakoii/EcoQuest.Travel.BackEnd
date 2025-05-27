using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class Trip
{
    public Guid TripId { get; set; }

    public Guid UserId { get; set; }

    public string? TripName { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? NumberOfPeople { get; set; }

    public decimal? TotalEstimatedCost { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual User User { get; set; } = null!;
}
