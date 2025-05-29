using BackEnd.Controllers;

namespace BackEnd.DTOs.User;

public class InsertUserResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}