using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

/// <summary>
/// Ecq300SelectPaymentBookingResponse - Response for select payment booking detail
/// </summary>
public class Ecq300SelectPaymentBookingResponse : AbstractApiResponse<Ecq300SelectPaymentBookingEntity>
{
    public override Ecq300SelectPaymentBookingEntity Response { get; set; } = null!;
}

/// <summary>
/// Ecq300SelectPaymentBookingEntity - Payment booking detail entity
/// </summary>
public class Ecq300SelectPaymentBookingEntity
{
    /// <summary>
    /// Payment ID
    /// </summary>
    public Guid PaymentId { get; set; }

    /// <summary>
    /// Trip ID
    /// </summary>
    public Guid TripId { get; set; }

    /// <summary>
    /// Trip Name
    /// </summary>
    public string? TripName { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User First Name
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// User Last Name
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// User Email
    /// </summary>
    public string? UserEmail { get; set; }

    /// <summary>
    /// Payment Amount
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Payment Method
    /// </summary>
    public string Method { get; set; } = null!;

    /// <summary>
    /// Payment Status
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// Transaction Code
    /// </summary>
    public string? TransactionCode { get; set; }

    /// <summary>
    /// Paid At
    /// </summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>
    /// Payment Created At
    /// </summary>
    public DateTime PaymentCreatedAt { get; set; }

    /// <summary>
    /// Trip Start Date
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// Trip End Date
    /// </summary>
    public DateOnly? EndDate { get; set; }

    /// <summary>
    /// Number of People
    /// </summary>
    public int? NumberOfPeople { get; set; }

    /// <summary>
    /// Total Estimated Cost
    /// </summary>
    public decimal? TotalEstimatedCost { get; set; }

    /// <summary>
    /// Starting Point Address
    /// </summary>
    public string StartingPointAddress { get; set; } = null!;

    /// <summary>
    /// Trip Description
    /// </summary>
    public string? TripDescription { get; set; }

    /// <summary>
    /// Trip Status
    /// </summary>
    public byte TripStatus { get; set; }
}
