using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class VwRestaurantRating
{
    public Guid RatingId { get; set; }

    public Guid RestaurantId { get; set; }

    public Guid UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
