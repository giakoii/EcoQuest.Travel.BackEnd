using System;
using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110InsertTripScheduleRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "TripId is required")]
    public Guid TripId { get; set; }
    
    [Required(ErrorMessage = "TripScheduleDetails is required")]
    public List<Ecq110InsertTripScheduleRequestDeatil> TripScheduleDetails { get; set; }
   
}

public class Ecq110InsertTripScheduleRequestDeatil
{
    [Required(ErrorMessage = "ScheduleDate is required")]
    public DateOnly ScheduleDate { get; set; }

    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }
    
    public string? Description { get; set; }
    
    public decimal? EstimatedCost { get; set; }
    
    [Required(ErrorMessage = "StartTime is required")]
    public TimeOnly StartTime { get; set; }
    
    [Required(ErrorMessage = "EndTime is required")]
    public TimeOnly? EndTime { get; set; }
    
    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; }
    
    public Guid? ServiceId { get; set; }
}
