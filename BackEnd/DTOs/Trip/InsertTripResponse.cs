using BackEnd.Controllers;

namespace BackEnd.DTOs.Trip;

public class InsertTripResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}