using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

/// <summary>
/// Ecq300SelectPaymentBookingRequest - Request to select payment booking detail
/// </summary>
public class Ecq300SelectPaymentBookingRequest : AbstractApiRequest
{
    /// <summary>
    /// Payment ID
    /// </summary>
    public Guid PaymentId { get; set; }
}
