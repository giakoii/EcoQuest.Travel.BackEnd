using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110SelectPaymentBookingResponse : AbstractApiResponse<Ecq110SelectPaymentBookingEntity>
{
    public override Ecq110SelectPaymentBookingEntity Response { get; set; }
}

public class Ecq110SelectPaymentBookingEntity
{
}