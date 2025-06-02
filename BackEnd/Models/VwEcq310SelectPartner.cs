using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class VwEcq310SelectPartner
{
    public Guid PartnerId { get; set; }

    public Guid AccountId { get; set; }

    public string? CompanyName { get; set; }

    public string ContactName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Description { get; set; }

    public bool? Verified { get; set; }

    public DateTime CreatedAt { get; set; }
}
