using BackEnd.DTOs.Ecq100;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.SystemClient;
using BackEnd.Utils;
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
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.UpdatedAt),
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

    /// <summary>
    /// Inserts a new comment.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq100InsertCommentResponse> InsertComment(Ecq100InsertCommentRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq100InsertCommentResponse { Success = false };
        
        // Create new comment entity
        var comment = new Comment
        {
            BlogId = request.BlogId,
            Content = request.Content,
            UserId = Guid.Parse(identityEntity.UserId),
            ParentCommentId = request.ParentCommentId,
        };
        
        // Save changes
        await _commentRepository.AddAsync(comment);
        await _commentRepository.SaveChangesAsync(identityEntity.Email);
        
        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }

    /// <summary>
    /// Updates an existing comment.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq100UpdateCommentResponse> UpdateComment(Ecq100UpdateCommentRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq100UpdateCommentResponse { Success = false };
        
        // Find comment by ID
        var comment = await _commentRepository.Find(x => x.CommentId == request.CommentId && x.IsActive == true).FirstOrDefaultAsync();
        if (comment == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.CommentNotFound);
            return response;
        }
        
        // Check if the user is the owner of the comment
        if (comment.UserId != Guid.Parse(identityEntity.UserId))
        {
            response.SetMessage(MessageId.I00000, "You are not authorized to update this comment");
            return response;
        }
        
        comment.Content = request.Content;
        
        // Save changes
        _commentRepository.Update(comment);
        await _commentRepository.SaveChangesAsync(identityEntity.Email);
        
        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }

    /// <summary>
    /// Deletes a comment.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq100DeleteCommentResponse> DeleteComment(Ecq100DeleteCommentRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq100DeleteCommentResponse { Success = false };
        
        // Find comment by ID
        var comment = await _commentRepository.Find(x => x.CommentId == request.CommentId && x.IsActive == true).FirstOrDefaultAsync();
        if (comment == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.CommentNotFound); 
            return response;
        }
        
        // Check if the user is the owner of the comment
        if (comment.UserId != Guid.Parse(identityEntity.UserId))
        {
            response.SetMessage(MessageId.I00000, "You are not authorized to update this comment");
            return response;
        }
        
        // Save changes
        await _commentRepository.UpdateAsync(comment);
        await _commentRepository.SaveChangesAsync(identityEntity.Email, true);
        
        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }
}

