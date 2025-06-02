using BackEnd.Controllers.V1.User;
using BackEnd.DTOs.Account;
using BackEnd.DTOs.Ecq310;
using BackEnd.DTOs.User;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IUserService
{
    Task<LoginResponse> AuthLogin(AuthRequest request);
    
    Task<Ecq010InsertUserResponse> InsertUser(Ecq010InsertUserRequest request);

    Task<Ecq010InsertUserResponse> InsertUser(string email, string firstName, string lastName, Guid roleId);

    Task<Ecq010InsertUserVerifyResponse> InsertUserVerify(Ecq010InsertUserVerifyRequest request);
    
    Task<Ecq300UpdateUserResponse> UpdateUser(Ecq300UpdateUserRequest request, IdentityEntity identityEntity);
    
    Task<Ecq300SelectUserResponse> SelectUser(Ecq300SelectUserRequest request, IdentityEntity identityEntity);
    
    Task<LoginResponse> GoogleLogin(string email, string firstName, string lastName, string pictureUrl);
}