using BackEnd.Controllers;

namespace BackEnd.Controllers.V1.User;

public class Ecq300UpdateUserResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}