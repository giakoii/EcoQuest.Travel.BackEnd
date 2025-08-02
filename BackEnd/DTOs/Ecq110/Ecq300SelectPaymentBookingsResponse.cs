using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq300SelectPaymentBookingsResponse : AbstractApiResponse<List<Ecq300SelectPaymentBookingsEntity>>
{
    public override List<Ecq300SelectPaymentBookingsEntity> Response { get; set; }
}

public class Ecq300SelectPaymentBookingsEntity
{
    public Guid PaymentId { get; set; }

    public Guid TripId { get; set; }

    public decimal Amount { get; set; }

    public string Method { get; set; } = null!;

    public string Status { get; set; } = null!;
    
    public DateTime? PaidAt { get; set; }
}