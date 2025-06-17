using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110SelectTripSchedulesResponse : AbstractApiResponse<List<Ecq110TripScheduleEntity>>
{
    public override List<Ecq110TripScheduleEntity> Response { get; set; }
}
