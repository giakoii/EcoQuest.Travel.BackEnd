using BackEnd.Controllers.V1.User;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IUserService
{
    Task<InsertUserResponse> InsertUser(InsertUserRequest request);

    Task<InsertUserVerifyResponse> InsertUserVerify(InsertUserVerifyRequest request);
    
    Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, IdentityEntity identityEntity);
    
    Task<SelectUserResponse> SelectUser(SelectUserRequest request, IdentityEntity identityEntity);
}