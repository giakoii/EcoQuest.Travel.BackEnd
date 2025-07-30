using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class VwPaymentBookingTrip
{
    public Guid PaymentId { get; set; }

    public Guid TripId { get; set; }

    public string? TripName { get; set; }

    public Guid UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? UserEmail { get; set; }

    public decimal Amount { get; set; }

    public string Method { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? TransactionCode { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime PaymentCreatedAt { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? NumberOfPeople { get; set; }

    public decimal? TotalEstimatedCost { get; set; }

    public string StartingPointAddress { get; set; } = null!;

    public string? TripDescription { get; set; }

    public byte TripStatus { get; set; }
}
