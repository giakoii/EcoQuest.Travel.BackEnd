using System.ComponentModel.DataAnnotations;

namespace BackEnd.Controllers.V1.User;

public class Ecq010InsertUserVerifyRequest
{
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Key is required")]
    public string Key { get; set; }
}