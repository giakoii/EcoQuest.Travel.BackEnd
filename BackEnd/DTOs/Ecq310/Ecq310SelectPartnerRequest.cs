using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq310;

public class Ecq310SelectPartnerRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "Partner ID is required")]
    public Guid PartnerId { get; set; }
}