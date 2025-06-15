using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;
using Microsoft.AspNetCore.Http;

namespace BackEnd.DTOs.Ecq230;

public class Ecq230UpdateAttractionRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "Attraction ID is required.")]
    public Guid AttractionId { get; set; }
    
    [Required(ErrorMessage = "Attraction name is required.")]
    public string AttractionName { get; set; } = null!;

    [Required(ErrorMessage = "Attraction type is required.")]
    public string AttractionType { get; set; } = null!;

    [Required(ErrorMessage = "Ticket price is required.")]
    public decimal TicketPrice { get; set; }

    [Required(ErrorMessage = "Open time is required.")]
    public TimeOnly OpenTime { get; set; }

    [Required(ErrorMessage = "Close time is required.")]
    public TimeOnly CloseTime { get; set; }

    [Required(ErrorMessage = "Destination ID is required.")]
    public Guid DestinationId { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; } = null!;

    public bool? GuideAvailable { get; set; }

    public int? AgeLimit { get; set; }

    public int? DurationMinutes { get; set; }
    
    // Images are optional for update
    public List<IFormFile>? AttractionImages { get; set; }
    
    public List<string>? ImagesToRemove { get; set; }
}
