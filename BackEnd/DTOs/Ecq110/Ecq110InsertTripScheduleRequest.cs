using System;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110InsertTripScheduleRequest : AbstractApiRequest
{
    public Guid TripId { get; set; }
    public DateOnly ScheduleDate { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public string? Location { get; set; }
}
