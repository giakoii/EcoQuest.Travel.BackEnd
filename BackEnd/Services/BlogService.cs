using BackEnd.DTOs.Ecq100;
using BackEnd.Logics;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.SystemClient;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services;

public class BlogService : IBlogService
{
    private readonly IBaseRepository<Blog, Guid> _blogRepository;
    private readonly IBaseRepository<Image, Guid> _imageRepository;
    private readonly CloudinaryLogic _cloudinary;

    public BlogService(
        IBaseRepository<Blog, Guid> blogRepository, 
        IBaseRepository<Image, Guid> imageRepository,
        CloudinaryLogic cloudinary)
    {
        _blogRepository = blogRepository;
        _imageRepository = imageRepository;
        _cloudinary = cloudinary;
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
           CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.CreatedAt),
           UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.UpdatedAt),
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
        
        // Select blog images
        var blogImageUrls = await _imageRepository
            .GetView<VwImage>(x => x.EntityId == requestBlogId && 
                                x.EntityType == ConstantEnum.EntityImage.Blog.ToString())
            .Select(x => x.ImageUrl)
            .ToListAsync();
        
        response.Response.BlogImages = blogImageUrls!;
        
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
        var blogs = await _blogRepository.GetView<VwBlog>()
            .Select(x => new Ecq100SelectBlogsEntity
            {
                BlogId = x.BlogId,
                Title = x.Title,
                Content = x.Content,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.UpdatedAt),
                AuthorId = x.AuthorId,
                AuthorFirstName = x.AuthorFirstName,
                AuthorLastName = x.AuthorLastName,
                DestinationName = x.DestinationName,
                DestinationId = x.DestinationId,
            }).ToListAsync();

        response.Response = blogs;
        
        // Fetch images for each blog
        foreach (var blog in response.Response)
        {
            var blogImageUrls = await _imageRepository
                .GetView<VwImage>(x => x.EntityId == blog.BlogId && 
                                    x.EntityType == ConstantEnum.EntityImage.Blog.ToString())
                .Select(x => x.ImageUrl)
                .ToListAsync();
            
            blog.BlogImages = blogImageUrls!;
        }
        
        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }
    
    /// <summary>
    /// Insert a new blog
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq100InsertBlogResponse> InsertBlog(Ecq100InsertBlogRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq100InsertBlogResponse { Success = false };
        
        // Begin transaction
        await _blogRepository.ExecuteInTransactionAsync(async () =>
        {
            // Create new blog entity
            var blog = new Blog
            {
                BlogId = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                AuthorId = Guid.Parse(identityEntity.UserId),
                DestinationId = request.DestinationId,
            };
            
            // Add blog to repository
            await _blogRepository.AddAsync(blog);
            await _blogRepository.SaveChangesAsync(identityEntity.Email);
            
            // Upload images if provided
            if (request.BlogImages != null && request.BlogImages.Count > 0)
            {
                foreach (var image in request.BlogImages)
                {
                    // Upload image to Cloudinary and get URL
                    var imageUrl = await _cloudinary.UploadImageAsync(image);
                    
                    // Create new image entity
                    var blogImage = new Image
                    {
                        EntityId = blog.BlogId,
                        ImageUrl = imageUrl,
                        EntityType = ConstantEnum.EntityImage.Blog.ToString(),
                    };
                    // Add image to repository
                    await _imageRepository.AddAsync(blogImage);
                }
                
                // Save images
                await _imageRepository.SaveChangesAsync(identityEntity.Email);
            }
            
            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }
}
