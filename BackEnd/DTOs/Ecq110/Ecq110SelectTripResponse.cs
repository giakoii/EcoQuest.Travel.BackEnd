using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110SelectTripResponse : AbstractApiResponse<Ecq110TripEntity>
{
    public override Ecq110TripEntity Response { get; set; }
}

public class Ecq110TripEntity
{
    public Guid TripId { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string TripName { get; set; } = null!;
    public string? Description { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public int? NumberOfPeople { get; set; }
    public decimal? TotalEstimatedCost { get; set; }
    public byte Status { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    
    public List<Ecq110SelectTripDestination> Destinations { get; set; }
}

public class Ecq110SelectTripDestination
{
    public Guid DestinationId { get; set; }
    
    public string DestinationName { get; set; } = null!;
}
