using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class VwHotelRoom
{
    public Guid RoomId { get; set; }

    public Guid HotelId { get; set; }

    public string RoomType { get; set; } = null!;

    public decimal PricePerNight { get; set; }

    public int MaxGuests { get; set; }

    public string? Description { get; set; }

    public bool? IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool? IsActive { get; set; }
}
