using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class VwTripSchedule
{
    public Guid ScheduleId { get; set; }

    public Guid TripId { get; set; }

    public Guid? UserId { get; set; }

    public string? TripName { get; set; }

    public DateOnly ScheduleDate { get; set; }

    public string ScheduleTitle { get; set; } = null!;

    public string? ScheduleDescription { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? Location { get; set; }

    public Guid? ServiceId { get; set; }

    public string? ServiceType { get; set; }

    public decimal? EstimatedCost { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }
}
