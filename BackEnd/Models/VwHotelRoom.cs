﻿using System;
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

    public int? Area { get; set; }

    public string? BedType { get; set; }

    public int? NumberOfBeds { get; set; }

    public int? NumberOfRoomsAvailable { get; set; }

    public bool? HasPrivateBathroom { get; set; }

    public bool? HasAirConditioner { get; set; }

    public bool? HasWifi { get; set; }

    public bool? HasBreakfast { get; set; }

    public bool? HasTv { get; set; }

    public bool? HasMinibar { get; set; }

    public bool? HasBalcony { get; set; }

    public bool? HasWindow { get; set; }

    public bool? IsRefundable { get; set; }

    public DateTime? FreeCancellationUntil { get; set; }

    public bool? SmokingAllowed { get; set; }

    public TimeOnly? CheckinTime { get; set; }

    public TimeOnly? CheckoutTime { get; set; }

    public string? SpecialNote { get; set; }
}
