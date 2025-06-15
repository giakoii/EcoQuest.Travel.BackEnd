using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;
using Microsoft.AspNetCore.Http;

namespace BackEnd.DTOs.Ecq200;

public class Ecq200UpdateDestinationRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "DestinationId is required.")]
    public Guid DestinationId { get; set; }
    
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = null!;
    
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }
    
    [StringLength(200, ErrorMessage = "AddressLine cannot exceed 200 characters.")]
    public string? AddressLine { get; set; }
    
    [StringLength(100, ErrorMessage = "Ward cannot exceed 100 characters.")]
    public string? Ward { get; set; }
    
    [StringLength(100, ErrorMessage = "District cannot exceed 100 characters.")]
    public string? District { get; set; }
    
    [StringLength(100, ErrorMessage = "Province cannot exceed 100 characters.")]
    public string? Province { get; set; }
    
    public List<IFormFile>? NewDestinationImages { get; set; }
    
    public List<string>? ImagesToRemove { get; set; }
}
