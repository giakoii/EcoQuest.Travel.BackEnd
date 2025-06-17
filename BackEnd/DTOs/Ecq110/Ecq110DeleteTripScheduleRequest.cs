using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110DeleteTripScheduleRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "ScheduleId is required.")]
    public Guid ScheduleId { get; set; }
}
