﻿using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class Destination
{
    public Guid DestinationId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? AddressLine { get; set; }

    public string? Ward { get; set; }

    public string? District { get; set; }

    public string? Province { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual ICollection<AttractionDetail> AttractionDetails { get; set; } = new List<AttractionDetail>();

    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();

    public virtual ICollection<RestaurantDetail> RestaurantDetails { get; set; } = new List<RestaurantDetail>();

    public virtual ICollection<TripDestination> TripDestinations { get; set; } = new List<TripDestination>();
}
