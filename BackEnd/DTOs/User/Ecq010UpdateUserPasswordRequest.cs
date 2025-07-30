using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.User;

/// <summary>
/// Request DTO for updating user password
/// </summary>
public class Ecq010UpdateUserPasswordRequest : AbstractApiRequest
{

    [Required]
    public string OldPassword { get; set; } = string.Empty;

    [Required]
    public string NewPassword { get; set; } = string.Empty;
}

