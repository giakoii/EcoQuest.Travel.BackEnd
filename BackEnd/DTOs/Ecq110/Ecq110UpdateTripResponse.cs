using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110UpdateTripResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}
