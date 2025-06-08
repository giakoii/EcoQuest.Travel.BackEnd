using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq220;

public class Ecq220SelectRestaurantRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "RestaurantId is required")]
    public Guid RestaurantId { get; set; }
}
