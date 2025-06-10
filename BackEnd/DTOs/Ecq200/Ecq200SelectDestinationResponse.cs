using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq200;

public class Ecq200SelectDestinationResponse : AbstractApiResponse<Ecq200DestinationDetailEntity>
{
    public override Ecq200DestinationDetailEntity Response { get; set; }
}

public class Ecq200DestinationDetailEntity
{
    public Guid DestinationId { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public string? AddressLine { get; set; }
    
    public string? Ward { get; set; }
    
    public string? District { get; set; }
    
    public string? Province { get; set; }
    
    public string CreatedAt { get; set; }
    
    public string UpdatedAt { get; set; }
    
    public List<string>? DestinationImages { get; set; }
}
