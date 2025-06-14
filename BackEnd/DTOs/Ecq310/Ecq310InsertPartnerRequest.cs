using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq310;

public class Ecq310InsertPartnerRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Phone number is invalid")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Company name is required")]
    public string? CompanyName { get; set; }

    [Required(ErrorMessage = "Contact name is required")]
    public string ContactName { get; set; } = null!;
    
    [Required(ErrorMessage = "Description is required")]
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Partner type is required")]
    public List<byte> PartnerType { get; set; }
}