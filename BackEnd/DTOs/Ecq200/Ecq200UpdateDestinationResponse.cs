using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq200;

public class Ecq200UpdateDestinationResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}
