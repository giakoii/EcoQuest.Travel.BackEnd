using BackEnd.DTOs.Ecq100;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface ICommentService
{
    Task<Ecq100SelectCommentsResponse> SelectComments(Guid blogId);
    
    Task<Ecq100InsertCommentResponse> InsertComment(Ecq100InsertCommentRequest request, IdentityEntity identityEntity);

    Task<Ecq100UpdateCommentResponse> UpdateComment(Ecq100UpdateCommentRequest request, IdentityEntity identityEntity);


    Task<Ecq100DeleteCommentResponse> DeleteComment(Ecq100DeleteCommentRequest request, IdentityEntity identityEntity);

}
