using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class Booking
{
    public Guid BookingId { get; set; }

    public Guid UserId { get; set; }

    public Guid TripId { get; set; }

    public Guid? ServiceId { get; set; }

    public string? ServiceType { get; set; }

    public DateOnly ScheduleDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public DateOnly? CheckinDate { get; set; }

    public DateOnly? CheckoutDate { get; set; }

    public decimal TotalCost { get; set; }

    public int NumberOfGuests { get; set; }

    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Trip Trip { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
