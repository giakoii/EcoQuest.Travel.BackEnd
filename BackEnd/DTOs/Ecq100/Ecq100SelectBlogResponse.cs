using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100SelectBlogResponse : AbstractApiResponse<Ecq100SelectBlogEntity>
{
    public override Ecq100SelectBlogEntity Response { get; set; }
}

public class Ecq100SelectBlogEntity
{
    public Guid BlogId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid AuthorId { get; set; }

    public string AuthorFirstName { get; set; } = null!;

    public string AuthorLastName { get; set; } = null!;

    public string? AuthorAvatar { get; set; }

    public Guid? DestinationId { get; set; }

    public string? DestinationName { get; set; }

    public string? AddressLine { get; set; }

    public string? Ward { get; set; }

    public string? District { get; set; }

    public string? Province { get; set; }
    
    public List<string>? BlogImages { get; set; }
}
