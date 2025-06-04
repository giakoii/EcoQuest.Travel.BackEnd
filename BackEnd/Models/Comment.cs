using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class Comment
{
    public Guid CommentId { get; set; }

    public Guid BlogId { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = null!;

    public Guid? ParentCommentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual Blog Blog { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
