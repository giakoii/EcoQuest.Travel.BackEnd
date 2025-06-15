using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;
using Microsoft.AspNetCore.Http;

namespace BackEnd.DTOs.Ecq210;

public class Ecq210UpdateHotelRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "HotelId is required.")]
    public Guid HotelId { get; set; }
    
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = null!;
    
    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; }
    
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }
    
    [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
    public string? Address { get; set; }
    
    [StringLength(11, ErrorMessage = "PhoneNumber cannot exceed 100 characters.")]
    public string PhoneNumber { get; set; }
    
    public Guid? DestinationId { get; set; }
    
    public List<IFormFile>? NewHotelImages { get; set; }
    
    public List<string>? ImagesToRemove { get; set; }
}
