using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100DeleteCommentRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "CommentId is required.")]
    public Guid CommentId { get; set; }
}
