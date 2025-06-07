using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class TripSchedule
{
    public Guid ScheduleId { get; set; }

    public Guid TripId { get; set; }

    public DateOnly ScheduleDate { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? Location { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual Trip Trip { get; set; } = null!;
}
