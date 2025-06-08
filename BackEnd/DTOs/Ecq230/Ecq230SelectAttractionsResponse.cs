using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq230;

public class Ecq230SelectAttractionsResponse : AbstractApiResponse<List<Ecq230AttractionEntity>>
{
    public override List<Ecq230AttractionEntity> Response { get; set; }
}

public class Ecq230AttractionEntity
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
