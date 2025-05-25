using BackEnd.Controllers.V1.User;

namespace BackEnd.Services;

public interface IUserService
{
    Task<InsertUserResponse> InsertUser(InsertUserRequest request);

    Task<InsertUserVerifyResponse> InsertUserVerify(InsertUserVerifyRequest request);
}