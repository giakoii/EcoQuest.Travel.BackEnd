using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100InsertCommentRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "BlogId is required.")]
    public Guid BlogId { get; set; }
    
    [Required(ErrorMessage = "Content is required.")]
    public string Content { get; set; } = null!;
    
    public Guid? ParentCommentId { get; set; }
}