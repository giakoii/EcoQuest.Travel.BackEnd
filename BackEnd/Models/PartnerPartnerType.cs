using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class PartnerPartnerType
{
    public Guid PartnerId { get; set; }

    public Guid TypeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual Partner Partner { get; set; } = null!;

    public virtual PartnerType Type { get; set; } = null!;
}
