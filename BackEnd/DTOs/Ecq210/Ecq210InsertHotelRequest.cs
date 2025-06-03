using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq210;

public class Ecq210InsertHotelRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "Hotel Name is required")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    [Required(ErrorMessage = "DestinationId is required")]
    public Guid DestinationId { get; set; }
}