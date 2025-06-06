using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq211;

public class Ecq211InsertHotelRoomRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "HotelId is required.")]
    public Guid HotelId { get; set; }
    
    [Required(ErrorMessage = "Room type is required.")]
    public string RoomType { get; set; } = null!;
    
    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; } = null!;
    
    [Required(ErrorMessage = "MaxGuests is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "MaxGuests must be greater than 0.")]
    public int MaxGuests { get; set; }
    
    [Required(ErrorMessage = "PricePerNight is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "PricePerNight must be greater than 0.")]
    public decimal PricePerNight { get; set; }
    
    public List<IFormFile>? HotelRoomImages { get; set; }

    [Required(ErrorMessage = "Area is required.")]
    public int Area { get; set; }

    [Required(ErrorMessage = "Bed type is required.")]
    [Range(1, 10, ErrorMessage = "Invalid bed type. Valid values are: 1-10 or 99.")]
    [RegularExpression(@"^(1|2|3|4|5|6|7|8|9|10|99)$", ErrorMessage = "Invalid bed type. Please select from: Single, Twin, Double, Queen, King, SuperKing, Suite, Family, Bunk, Sofa, or Other.")]
    public int BedType { get; set; }

    [Required(ErrorMessage = "NumberOfBeds is required.")]
    public int? NumberOfBeds { get; set; }

    [Required(ErrorMessage = "NumberOfRoomsAvailable is required.")]
    public int NumberOfRoomsAvailable { get; set; }

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
