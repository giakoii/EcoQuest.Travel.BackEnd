using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100SelectBlogsResponse : AbstractApiResponse<List<Ecq100SelectBlogsEntity>>
{
    public override List<Ecq100SelectBlogsEntity> Response { get; set; }
}

public class Ecq100SelectBlogsEntity
{
    public Guid BlogId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public string CreatedAt { get; set; }

    public string UpdatedAt { get; set; }

    public Guid AuthorId { get; set; }

    public string AuthorFirstName { get; set; } = null!;

    public string AuthorLastName { get; set; } = null!;
    
    public List<Guid> DestinationId { get; set; }

    public List<string> DestinationName { get; set; }
    
    public List<string>? BlogImages { get; set; }
}
