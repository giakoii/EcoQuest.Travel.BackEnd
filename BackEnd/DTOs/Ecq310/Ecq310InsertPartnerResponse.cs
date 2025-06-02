using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq310;

public class Ecq310InsertPartnerResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}