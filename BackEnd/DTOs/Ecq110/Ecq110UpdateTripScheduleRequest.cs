using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110UpdateTripScheduleRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "ScheduleId is required.")]
    public Guid ScheduleId { get; set; }
    
    [Required(ErrorMessage = "TripId is required.")]
    public Guid TripId { get; set; }
    
    [Required(ErrorMessage = "ScheduleDate is required.")]
    public DateTime ScheduleDate { get; set; }
    
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
    public string Title { get; set; }
    
    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string Description { get; set; }
    
    public TimeSpan? StartTime { get; set; }
    
    public TimeSpan? EndTime { get; set; }
    
    [StringLength(200, ErrorMessage = "Location cannot be longer than 200 characters.")]
    public string Location { get; set; }
}
