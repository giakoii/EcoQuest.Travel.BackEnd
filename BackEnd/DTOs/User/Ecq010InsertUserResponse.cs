using BackEnd.Controllers;

namespace BackEnd.DTOs.User;

public class Ecq010InsertUserResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}