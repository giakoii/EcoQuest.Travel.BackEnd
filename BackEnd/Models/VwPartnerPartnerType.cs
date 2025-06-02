using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class VwPartnerPartnerType
{
    public Guid PartnerId { get; set; }

    public Guid TypeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? TypeName { get; set; }
}
