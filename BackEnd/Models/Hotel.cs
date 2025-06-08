using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class Hotel
{
    public Guid HotelId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public Guid? DestinationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsActive { get; set; }

    public Guid? OwnerId { get; set; }

    public virtual Destination? Destination { get; set; }

    public virtual ICollection<HotelRating> HotelRatings { get; set; } = new List<HotelRating>();

    public virtual ICollection<HotelRoom> HotelRooms { get; set; } = new List<HotelRoom>();

    public virtual Partner? Owner { get; set; }
}
