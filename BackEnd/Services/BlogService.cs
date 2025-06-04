using BackEnd.DTOs.Ecq100;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.Utils.Const;

namespace BackEnd.Services;

public class BlogService : IBlogService
{
    private readonly IBaseRepository<Blog, Guid> _blogRepository;

    public BlogService(IBaseRepository<Blog, Guid> blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public async Task<Ecq100SelectBlogResponse> SelectBlog(Guid requestBlogId)
    {
        var response = new Ecq100SelectBlogResponse {Success = false};
        
        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }

    public Task<Ecq100SelectBlogsResponse> SelectBlogs()
    {
        throw new NotImplementedException();
    }
}