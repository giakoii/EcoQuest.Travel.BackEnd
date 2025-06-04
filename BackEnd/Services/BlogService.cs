using BackEnd.DTOs.Ecq100;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services;

public class BlogService : IBlogService
{
    private readonly IBaseRepository<Blog, Guid> _blogRepository;

    public BlogService(IBaseRepository<Blog, Guid> blogRepository)
    {
        _blogRepository = blogRepository;
    }

    /// <summary>
    /// Select blog by requestBlogId
    /// </summary>
    /// <param name="requestBlogId"></param>
    /// <returns></returns>
    public async Task<Ecq100SelectBlogResponse> SelectBlog(Guid requestBlogId)
    {
        var response = new Ecq100SelectBlogResponse {Success = false};
        
        // Select blog
       response.Response = (await _blogRepository.GetView<VwBlog>(x => x.BlogId == requestBlogId).Select(x => new Ecq100SelectBlogEntity
       {
           BlogId = x.BlogId,
           Title = x.Title,
           Content = x.Content,
           CreatedAt = x.CreatedAt,
           UpdatedAt = x.UpdatedAt,
           AuthorId = x.AuthorId,
           AuthorFirstName = x.AuthorFirstName,
           AuthorLastName = x.AuthorLastName,
           DestinationName = x.DestinationName,
           DestinationId = x.DestinationId,
           District = x.District,
           Province = x.Province,
           Ward = x.Ward,
           AddressLine = x.AddressLine,
           AuthorAvatar = x.AuthorAvatar,
       }).FirstOrDefaultAsync())!;
        if (response.Response == null)
        {
            response.SetMessage(MessageId.E00000, CommonMessages.BlogNotFound);
            return response;
        }
        
        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }

    /// <summary>
    /// Select blogs
    /// </summary>
    /// <returns></returns>
    public async Task<Ecq100SelectBlogsResponse> SelectBlogs()
    {
        var response = new Ecq100SelectBlogsResponse {Success = false};
        
        // Select blogs
        response.Response = await _blogRepository.GetView<VwBlog>()
            .Select(x => new Ecq100SelectBlogsEntity
            {
                BlogId = x.BlogId,
                Title = x.Title,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                AuthorId = x.AuthorId,
                AuthorFirstName = x.AuthorFirstName,
                AuthorLastName = x.AuthorLastName,
                DestinationName = x.DestinationName,
                DestinationId = x.DestinationId,
            }).ToListAsync();
        
        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }
}