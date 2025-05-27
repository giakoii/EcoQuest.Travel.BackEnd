using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class Blog
{
    public Guid BlogId { get; set; }

    public Guid AuthorId { get; set; }

    public Guid? DestinationId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual Destination? Destination { get; set; }
}
