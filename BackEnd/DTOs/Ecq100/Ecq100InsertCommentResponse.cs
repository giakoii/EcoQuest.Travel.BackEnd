using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100InsertCommentResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}