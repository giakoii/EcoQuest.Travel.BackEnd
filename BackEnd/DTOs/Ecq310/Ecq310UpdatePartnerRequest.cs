using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq310;

public class Ecq310UpdatePartnerRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "PartnerId is required")]
    public Guid PartnerId { get; set; }

    [Required(ErrorMessage = "Company name is required")]
    public string? CompanyName { get; set; }

    [Required(ErrorMessage = "Contact name is required")]
    public string ContactName { get; set; } = null!;
    
    [Required(ErrorMessage = "Description is required")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Partner type is required")]
    public List<byte> PartnerType { get; set; } = null!;
}