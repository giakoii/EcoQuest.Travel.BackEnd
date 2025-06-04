using BackEnd.DTOs.Ecq100;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services;

public class CommentService : ICommentService
{
    private readonly IBaseRepository<Comment, Guid> _commentRepository;

    public CommentService(IBaseRepository<Comment, Guid> commentRepository)
    {
        _commentRepository = commentRepository;
    }

    /// <summary>
    /// Selects comments for a given blog ID.
    /// </summary>
    /// <param name="blogId"></param>
    /// <returns></returns>
    public async Task<Ecq100SelectCommentsResponse> SelectComments(Guid blogId)
    {
        var response = new Ecq100SelectCommentsResponse { Success = false };
        
        // Select comments
        response.Response = await _commentRepository.GetView<VwComment>(x => x.BlogId == blogId)
            .Select(x => new Ecq100SelectCommentsEntity
            {
                CommentId = x.CommentId,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                BlogTitle = x.BlogTitle,
                CommenterName = x.CommenterName,
                UserId = x.UserId,
                ParentCommentId = x.ParentCommentId,
            }).ToListAsync();
        
        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }
}