using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110PaymentCallbackResponse : AbstractApiResponse<Ecq110PaymentCallbackEntity>
{
    public override Ecq110PaymentCallbackEntity Response { get; set; }
}

public class Ecq110PaymentCallbackEntity
{
    public string CheckoutUrl { get; set; }
    
    public string QrCode { get; set; }
}