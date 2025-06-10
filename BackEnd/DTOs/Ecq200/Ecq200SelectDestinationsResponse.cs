using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq200;

public class Ecq200SelectDestinationsResponse : AbstractApiResponse<List<Ecq200DestinationEntity>>
{
    public override List<Ecq200DestinationEntity> Response { get; set; }
}

public class Ecq200DestinationEntity
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
