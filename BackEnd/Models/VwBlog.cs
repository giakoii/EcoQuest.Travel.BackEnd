using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class VwBlog
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

    public Guid? TripId { get; set; }

    public string? TripName { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? DestinationId { get; set; }

    public string? DestinationName { get; set; }
}
