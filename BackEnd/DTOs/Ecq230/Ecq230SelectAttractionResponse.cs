using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq230;

public class Ecq230SelectAttractionResponse : AbstractApiResponse<Ecq230AttractionDetailEntity>
{
    public override Ecq230AttractionDetailEntity Response { get; set; }
}

public class Ecq230AttractionDetailEntity
{
    public Guid AttractionId { get; set; }

    public string? AttractionName { get; set; }

    public Guid PartnerId { get; set; }

    public string? AttractionType { get; set; }

    public decimal? TicketPrice { get; set; }

    public string OpenTime { get; set; }

    public string CloseTime { get; set; }

    public string CreatedAt { get; set; }

    public string UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public Guid? DestinationId { get; set; }

    public string? DestinationName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public bool? GuideAvailable { get; set; }

    public int? AgeLimit { get; set; }

    public int? DurationMinutes { get; set; }

    public decimal? AverageRating { get; set; }

    public int? TotalRatings { get; set; }
    
    public List<string>? AttractionImages { get; set; }
}
