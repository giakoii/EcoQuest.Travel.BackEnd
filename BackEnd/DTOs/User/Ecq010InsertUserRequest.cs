using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.User;

public class Ecq010InsertUserRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "Email is required"), EmailAddress(ErrorMessage = "Email is invalid")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    [RegularExpression(@"^(?=.*[0-9])(?=.*[^a-zA-Z0-9]).+$", ErrorMessage = "Password must contain at least one number and one special character")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "First name is required")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name cannot contain numbers or special characters")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name cannot contain numbers or special characters")]
    public string LastName { get; set; }
}