using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110InsertTripRequest : AbstractApiRequest
{
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
    
    
    public string? StartingPointAddress { get; set; }
    
    [Required(ErrorMessage = "At least one destination is required.")]
    public List<TripDestinationInfo> Destinations { get; set; }
}

public class TripDestinationInfo
{
    [Required(ErrorMessage = "DestinationId is required.")]
    public Guid DestinationId { get; set; }
    
    public string? Note { get; set; }
}
