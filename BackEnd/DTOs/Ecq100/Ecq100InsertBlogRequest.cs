using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;
using Microsoft.AspNetCore.Http;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100InsertBlogRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; } = null!;
    
    [Required(ErrorMessage = "Content is required.")]
    public string Content { get; set; } = null!;
    
    [Required(ErrorMessage = "TripId is required.")]
    public Guid TripId { get; set; }
    
    [Required(ErrorMessage = "Please insert at least one image about your trip.")]
    public List<IFormFile> BlogImages { get; set; }
}
