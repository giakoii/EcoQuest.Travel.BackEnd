using BackEnd.Controllers;

namespace BackEnd.DTOs.User;

public class InsertPartnerResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}