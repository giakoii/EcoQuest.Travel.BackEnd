using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110SelectTripSchedulesResponse : AbstractApiResponse<List<Ecq110TripSchedulesEntity>>
{
    public override List<Ecq110TripSchedulesEntity> Response { get; set; }
}

public class Ecq110TripSchedulesEntity
{
    public Guid TripId { get; set; }
    
    public string TripName { get; set; }
    
    public List<Ecq110TripScheduleDetail> TripScheduleDetails { get; set; }
}

public class Ecq110TripScheduleDetail
{
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

