using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100UpdateCommentRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "CommentId is required.")]
    public Guid CommentId { get; set; }
    
    [Required(ErrorMessage = "Content is required.")]
    public string Content { get; set; } = null!;
}
