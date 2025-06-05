using BackEnd.Controllers;
using Microsoft.AspNetCore.Http;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100InsertBlogRequest : AbstractApiRequest
{
    public string Title { get; set; } = null!;
    
    public string Content { get; set; } = null!;
    
    public Guid? DestinationId { get; set; }
    
    public List<IFormFile>? BlogImages { get; set; }
}
