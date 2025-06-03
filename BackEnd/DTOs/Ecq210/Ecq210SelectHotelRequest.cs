using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq210;

public class Ecq210SelectHotelRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "HotelId is required")]
    public Guid HotelId { get; set; }
}