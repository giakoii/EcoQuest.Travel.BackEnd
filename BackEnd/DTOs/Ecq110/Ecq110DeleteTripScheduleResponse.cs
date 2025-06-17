using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110DeleteTripScheduleResponse : AbstractApiResponse<bool>
{
    public override bool Response { get; set; }
}
