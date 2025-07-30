using BackEnd.Controllers;

namespace BackEnd.DTOs.User;

/// <summary>
/// Response DTO for updating user password
/// </summary>
public class Ecq010UpdateUserPasswordResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; } = string.Empty;
}
