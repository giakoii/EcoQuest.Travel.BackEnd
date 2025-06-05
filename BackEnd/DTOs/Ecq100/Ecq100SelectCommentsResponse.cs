using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100SelectCommentsResponse : AbstractApiResponse<List<Ecq100SelectCommentsEntity>>
{
    public override List<Ecq100SelectCommentsEntity> Response { get; set; }
}

public class Ecq100SelectCommentsEntity
{
    public Guid CommentId { get; set; }
    
    public string BlogTitle { get; set; } = null!;

    public Guid UserId { get; set; }

    public string CommenterName { get; set; } = null!;

    public string Content { get; set; } = null!;

    public Guid? ParentCommentId { get; set; }

    public string CreatedAt { get; set; }

    public string UpdatedAt { get; set; }
}