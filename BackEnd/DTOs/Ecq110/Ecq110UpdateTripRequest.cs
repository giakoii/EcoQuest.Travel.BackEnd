using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110UpdateTripRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "TripId is required.")]
    public Guid TripId { get; set; }
    
    [Required(ErrorMessage = "Trip name is required.")]
    public string TripName { get; set; } = null!;
    
    [Required(ErrorMessage = "Start date is required.")]
    public DateOnly StartDate { get; set; }
    
    [Required(ErrorMessage = "End date is required.")]
    public DateOnly EndDate { get; set; }
    
    [Required(ErrorMessage = "Number of people is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Number of people must be greater than 0.")]
    public int NumberOfPeople { get; set; }
    
    public decimal? TotalEstimatedCost { get; set; }
    
    public string? Description { get; set; }
    
    public byte Status { get; set; }
}
