using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq200;

public class Ecq200InsertDestinationResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}