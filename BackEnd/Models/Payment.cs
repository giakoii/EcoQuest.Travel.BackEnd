using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid BookingId { get; set; }

    public decimal Amount { get; set; }

    public string Method { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? TransactionCode { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
