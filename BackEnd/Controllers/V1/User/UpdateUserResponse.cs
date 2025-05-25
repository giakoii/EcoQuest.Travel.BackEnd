using BackEnd.Controllers;

namespace BackEnd.Controllers.V1.User;

public class UpdateUserResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}