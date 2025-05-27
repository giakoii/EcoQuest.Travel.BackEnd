using BackEnd.Controllers;

namespace BackEnd.Controllers.V1.User;

public class InsertUserResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}