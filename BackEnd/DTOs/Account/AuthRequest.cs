namespace BackEnd.DTOs.Account;

public class AuthRequest
{
    /// <summary>
    ///  UserName or Email
    /// </summary>
    public string Email { get; set; }
    
    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; }
}