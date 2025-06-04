using BackEnd.DTOs.Ecq100;

namespace BackEnd.Services;

public interface ICommentService
{
    Task<Ecq100SelectCommentsResponse> SelectComments(Guid blogId);
}