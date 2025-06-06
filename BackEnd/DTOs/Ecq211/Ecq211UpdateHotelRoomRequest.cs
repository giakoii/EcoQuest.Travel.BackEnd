using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq211;

public class Ecq211UpdateHotelRoomRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "RoomId is required.")]
    public Guid RoomId { get; set; }
    
    [Required(ErrorMessage = "Room type is required.")]
    public string RoomType { get; set; } = null!;
    
    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; } = null!;
    
    [Required(ErrorMessage = "MaxGuests is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "MaxGuests must be greater than 0.")]
    public int MaxGuests { get; set; }
    
    [Required(ErrorMessage = "PricePerNight is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "PricePerNight must be greater than 0.")]
    public decimal PricePerNight { get; set; }
    
    public bool IsAvailable { get; set; }
}
