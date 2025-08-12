using BackEnd.Controllers;
using BackEnd.DTOs.Ecq110;

namespace BackEnd.DTOs.User;

public class Ecq300PaymentPremierAccountResponse : AbstractApiResponse<Ecq110InsertPaymentEntity>
{
    public override Ecq110InsertPaymentEntity Response { get; set; }
}