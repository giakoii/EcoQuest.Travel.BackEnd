using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq211;

public class Ecq211SelectHotelRoomRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "RoomId is required.")]
    public Guid RoomId { get; set; }
}
