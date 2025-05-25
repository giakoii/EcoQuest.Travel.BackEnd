namespace BackEnd.Controllers.V1.Authentication;

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