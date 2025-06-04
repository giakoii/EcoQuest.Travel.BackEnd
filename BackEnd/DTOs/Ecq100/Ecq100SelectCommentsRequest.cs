using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100SelectCommentsRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "BlogId is required")]
    public Guid BlogId { get; set; }
}