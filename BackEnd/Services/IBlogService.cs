using BackEnd.DTOs.Ecq100;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IBlogService
{
    Task<Ecq100SelectBlogResponse> SelectBlog(Guid requestBlogId);
    
    Task<Ecq100SelectBlogsResponse> SelectBlogs();
    
    Task<Ecq100InsertBlogResponse> InsertBlog(Ecq100InsertBlogRequest request, IdentityEntity identityEntity);
}
