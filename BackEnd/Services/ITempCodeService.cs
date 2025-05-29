namespace BackEnd.Services;

public interface ITempCodeService
{
    Task SaveUserInfo(string code, TempUserInfo userInfo);
    Task<TempUserInfo?> GetUserInfo(string code);
    Task RemoveCode(string code);
}

public class TempUserInfo
{
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}