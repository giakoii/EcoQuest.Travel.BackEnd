using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class TripDestination
{
    public Guid TripDestinationId { get; set; }

    public Guid TripId { get; set; }

    public Guid DestinationId { get; set; }

    public int? OrderIndex { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Destination Destination { get; set; } = null!;

    public virtual Trip Trip { get; set; } = null!;
}
