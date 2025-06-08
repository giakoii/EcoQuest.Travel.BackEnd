using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq230;

public class Ecq230SelectAttractionRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "AttractionId is required")]
    public Guid AttractionId { get; set; }
}
