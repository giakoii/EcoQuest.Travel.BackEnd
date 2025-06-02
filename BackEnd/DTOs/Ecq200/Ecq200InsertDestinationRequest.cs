using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq200;

public class Ecq200InsertDestinationRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Address line is required")]
    public string? AddressLine { get; set; }

    [Required(ErrorMessage = "Ward is required")]
    public string? Ward { get; set; }

    [Required(ErrorMessage = "District is required")]
    public string? District { get; set; }

    [Required(ErrorMessage = "Province is required")]
    public string? Province { get; set; }

    public List<IFormFile>? DestinationImages { get; set; }
}