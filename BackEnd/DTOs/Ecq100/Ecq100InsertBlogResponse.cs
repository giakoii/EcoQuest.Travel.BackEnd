using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100InsertBlogResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}
