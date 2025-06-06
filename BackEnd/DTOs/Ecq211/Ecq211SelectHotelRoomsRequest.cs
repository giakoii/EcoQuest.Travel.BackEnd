using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq211;

public class Ecq211SelectHotelRoomsRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "HotelId is required.")]
    public Guid HotelId { get; set; }
}
