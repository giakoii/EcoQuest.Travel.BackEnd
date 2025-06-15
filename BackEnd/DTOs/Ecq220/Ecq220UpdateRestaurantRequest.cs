using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;
using Microsoft.AspNetCore.Http;

namespace BackEnd.DTOs.Ecq220;

public class Ecq220UpdateRestaurantRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "Restaurant ID is required.")]
    public Guid RestaurantId { get; set; }
    
    [Required(ErrorMessage = "Restaurant name is required.")]
    public string RestaurantName { get; set; } = null!;

    [Required(ErrorMessage = "Cuisine type is required.")]
    public string CuisineType { get; set; } = null!;

    public bool? HasVegetarian { get; set; }

    [Required(ErrorMessage = "Open time is required.")]
    public TimeOnly OpenTime { get; set; } 

    [Required(ErrorMessage = "Close time is required.")]
    public TimeOnly CloseTime { get; set; }

    [Required(ErrorMessage = "Minimum price is required.")]
    public decimal MinPrice { get; set; }

    [Required(ErrorMessage = "Maximum price is required.")]
    public decimal MaxPrice { get; set; }

    [Required(ErrorMessage = "Destination ID is required.")]
    public Guid DestinationId { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; } = null!;
    
    // Images are optional for update
    public List<IFormFile>? RestaurantImages { get; set; }
    
    public List<string>? ImagesToRemove { get; set; }

}
