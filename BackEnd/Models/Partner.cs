using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class Partner
{
    public Guid PartnerId { get; set; }

    public Guid AccountId { get; set; }

    public string? CompanyName { get; set; }

    public string ContactName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? Verified { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<AttractionDetail> AttractionDetails { get; set; } = new List<AttractionDetail>();

    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();

    public virtual ICollection<PartnerPartnerType> PartnerPartnerTypes { get; set; } = new List<PartnerPartnerType>();

    public virtual ICollection<RestaurantDetail> RestaurantDetails { get; set; } = new List<RestaurantDetail>();
}
