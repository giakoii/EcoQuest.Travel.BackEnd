using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110PaymentCallbackRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "TripId is required.")]
    public Guid TripId { get; set; }

    [Required(ErrorMessage = "Code is required.")]
    public string Code { get; set; }

    [Required(ErrorMessage = "Cancel is required.")]
    public bool Cancel { get; set; }
}