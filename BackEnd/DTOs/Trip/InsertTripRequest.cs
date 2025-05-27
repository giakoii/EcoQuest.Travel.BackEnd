using BackEnd.Controllers;

namespace BackEnd.DTOs.Trip;

public class InsertTripRequest : AbstractApiRequest
{
    public string? TripName { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? NumberOfPeople { get; set; }

    public decimal? TotalEstimatedCost { get; set; }
}