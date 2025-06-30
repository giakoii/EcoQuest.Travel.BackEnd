using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110InsertTripScheduleWithAiResponse : AbstractApiResponse<List<Ecq110InsertTripScheduleResponseDetail>>
{
    public override List<Ecq110InsertTripScheduleResponseDetail> Response { get; set; }
}

public class Ecq110InsertTripScheduleResponseDetail
{
    public DateOnly ScheduleDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; } = TimeOnly.MinValue;
    public TimeOnly? EndTime { get; set; } = null;
    public string Address { get; set; } = string.Empty;
    public decimal EstimatedCost { get; set; }
    
    public string ReasonEstimatedCost { get; set; }
    
    public Guid? ServiceId { get; set; }
    public string? ServiceType { get; set; }
}