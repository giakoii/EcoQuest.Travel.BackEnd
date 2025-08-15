using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq310;

public class Ecq310SelectPaymentRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "PaymentId is required.")]
    public Guid PaymentId { get; set; }
}