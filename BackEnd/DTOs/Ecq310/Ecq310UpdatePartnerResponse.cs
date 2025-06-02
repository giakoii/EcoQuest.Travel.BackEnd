using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq310;

public class Ecq310UpdatePartnerResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}