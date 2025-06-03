using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class Image
{
    public Guid ImageId { get; set; }

    public Guid EntityId { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsActive { get; set; }

    public string EntityType { get; set; } = null!;
}
