using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.Controllers.V1.User;

public class UpdateUserRequest : AbstractApiRequest
{
    public IFormFile? ImageUrl { get; set; }

    [Required(ErrorMessage = "BirthDate is required")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    public byte Gender { get; set; }
    
    [Required(ErrorMessage = "FirstName is required")]
    public string FirstName { get; set; } = null!;
    
    [Required(ErrorMessage = "LastName is required")]
    public string LastName { get; set; } = null!;
}