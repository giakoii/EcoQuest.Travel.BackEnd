using BackEnd.Controllers;
using BackEnd.Utils;

namespace BackEnd.DTOs.Ecq310;

public class Ecq310SelectPaymentsResponse : AbstractApiResponse<PagedResult<Ecq310SelectPaymentsEntity>>
{
    public override PagedResult<Ecq310SelectPaymentsEntity> Response { get; set; }
}

public class Ecq310SelectPaymentsEntity
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; }
    public DateTime? PaidAt { get; set; }
    public string Status { get; set; }
    public Guid TripId { get; set; }
    public string TripName { get; set; }
    public Guid UserId { get; set; }
}