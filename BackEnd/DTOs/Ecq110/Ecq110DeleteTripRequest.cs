using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110DeleteTripRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "TripId is required.")]
    public Guid TripId { get; set; }
}
