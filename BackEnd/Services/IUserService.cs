using BackEnd.Controllers;
using BackEnd.Controllers.V1.User;
using BackEnd.DTOs.Account;
using BackEnd.DTOs.User;
using BackEnd.SystemClient;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Services;

public interface IUserService
{
    Task<LoginResponse> AuthLogin(AuthRequest request);
    
    Task<InsertUserResponse> InsertUser(InsertUserRequest request);

    Task<InsertUserResponse> InsertUser(string email, string firstName, string lastName, Guid roleId);

    Task<InsertUserVerifyResponse> InsertUserVerify(InsertUserVerifyRequest request);
    
    Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, IdentityEntity identityEntity);
    
    Task<SelectUserResponse> SelectUser(SelectUserRequest request, IdentityEntity identityEntity);
    
    Task<LoginResponse> GoogleLogin(string email, string firstName, string lastName, string pictureUrl);
}