using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class PartnerType
{
    public Guid TypeId { get; set; }

    public string? TypeName { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual ICollection<PartnerPartnerType> PartnerPartnerTypes { get; set; } = new List<PartnerPartnerType>();
}
