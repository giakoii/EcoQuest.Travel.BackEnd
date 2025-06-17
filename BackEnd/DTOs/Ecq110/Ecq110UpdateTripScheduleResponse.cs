using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110UpdateTripScheduleResponse : AbstractApiResponse<bool>
{
    public override bool Response { get; set; }
}
