using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Chatbot;

public class Ecq110InsertTripScheduleWithAiRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "TripId is required")]
    public required Guid TripId { get; set; }
}