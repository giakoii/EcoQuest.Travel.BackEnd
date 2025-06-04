using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class VwComment
{
    public Guid CommentId { get; set; }

    public Guid BlogId { get; set; }

    public string BlogTitle { get; set; } = null!;

    public Guid UserId { get; set; }

    public string CommenterName { get; set; } = null!;

    public string Content { get; set; } = null!;

    public Guid? ParentCommentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
