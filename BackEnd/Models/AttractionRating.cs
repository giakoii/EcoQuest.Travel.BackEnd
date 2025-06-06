using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class AttractionRating
{
    public Guid RatingId { get; set; }

    public Guid PartnerId { get; set; }

    public Guid UserId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AttractionDetail Partner { get; set; } = null!;

    public virtual Partner PartnerNavigation { get; set; } = null!;
}
