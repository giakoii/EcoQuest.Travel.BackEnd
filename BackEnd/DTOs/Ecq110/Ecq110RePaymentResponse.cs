using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110RePaymentResponse : AbstractApiResponse<Ecq110RePaymentEntity>
{
    public override Ecq110RePaymentEntity Response { get; set; }
}

public class Ecq110RePaymentEntity
{
    public string PaymentUrl { get; set; }
}