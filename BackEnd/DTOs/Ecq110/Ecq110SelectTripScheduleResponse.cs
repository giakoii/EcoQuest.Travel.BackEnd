using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110SelectTripScheduleResponse : AbstractApiResponse<List<Ecq110TripScheduleEntity>>
{
    public override List<Ecq110TripScheduleEntity> Response { get; set; }
}

public class Ecq110TripScheduleEntity
{
    public Guid ScheduleId { get; set; }
    
    public Guid TripId { get; set; }
    
    public string ScheduleDate { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public string StartTime { get; set; }
    
    public string EndTime { get; set; }
    
    public string Location { get; set; }
    
    public string CreatedAt { get; set; }
    
    public string UpdatedAt { get; set; }
    
    public Guid? ServiceId { get; set; }
    
    public string ServiceType { get; set; }
    public decimal? EstimatedCost { get; set; }
}
