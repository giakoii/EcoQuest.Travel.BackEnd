using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class HotelRating
{
    public Guid RatingId { get; set; }

    public Guid UserId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public Guid? HotelId { get; set; }

    public virtual Hotel? Hotel { get; set; }

    public virtual User User { get; set; } = null!;
}
