using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110InsertPaymentResponse : AbstractApiResponse<Ecq110InsertPaymentEntity>
{
    public override Ecq110InsertPaymentEntity Response { get; set; }
}

public class Ecq110InsertPaymentEntity
{ 
    public string CheckoutUrl { get; set; }
    
    public string QrCode { get; set; }
}