using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Chatbot;

public class Ecq110InsertTripScheduleWithAiResponse : AbstractApiResponse<List<Ecq110InsertTripScheduleWithAiEntity>>
{
    public override List<Ecq110InsertTripScheduleWithAiEntity> Response { get; set; }
}

public class Ecq110InsertTripScheduleWithAiEntity
{
    public string ScheduleDate { get; set; }

    public string Title { get; set; }
    
    public string? Description { get; set; }
    
    public string StartTime { get; set; }
    
    public string EndTime { get; set; }
    
    public string Address { get; set; }
}