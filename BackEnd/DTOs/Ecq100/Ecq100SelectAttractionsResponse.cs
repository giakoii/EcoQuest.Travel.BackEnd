using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100SelectAttractionsResponse : AbstractApiResponse<List<Ecq100SelectAttractionsEntity>>
{
    public override List<Ecq100SelectAttractionsEntity> Response { get; set; }
}

public class Ecq100SelectAttractionsEntity
{
    public Guid AttractionId { get; set; }  
    
    public string? AttractionName { get; set; }
    
    public decimal? EntryFee { get; set; }
    
    public string CreatedAt { get; set; }
    
    public string? AttractionType { get; set; }
    
    public Guid? DestinationId { get; set; }
    
    public string? DestinationName { get; set; }
    
    public List<string>? AttractionImages { get; set; }
    
    public decimal? AverageRating { get; set; }
    
    public int? TotalRatings { get; set; }
}