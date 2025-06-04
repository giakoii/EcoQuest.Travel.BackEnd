using BackEnd.DTOs.Ecq100;

namespace BackEnd.Services;

public interface IBlogService
{
    Task<Ecq100SelectBlogResponse> SelectBlog(Guid requestBlogId);
    
    Task<Ecq100SelectBlogsResponse> SelectBlogs();
}