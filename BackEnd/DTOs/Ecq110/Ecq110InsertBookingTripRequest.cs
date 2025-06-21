using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110InsertBookingTripRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "TripId is required.")]
    public Guid TripId { get; set; }
    
    public List<BookingHotelRooms>? BookingHotelRooms { get; set; }
}

public class BookingHotelRooms
{
    public Guid? HotelRoomId { get; set; }
}