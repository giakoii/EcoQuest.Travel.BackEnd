using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class VwHotelBooking
{
    public Guid HotelBookingId { get; set; }

    public Guid BookingId { get; set; }

    public DateOnly CheckinDate { get; set; }

    public DateOnly CheckoutDate { get; set; }

    public Guid RoomId { get; set; }

    public int NumberOfRooms { get; set; }

    public decimal PricePerNight { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public bool IsActive { get; set; }
}
