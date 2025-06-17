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
    private readonly IBaseRepository<Trip, Guid> _tripRepository;
    private readonly CloudinaryLogic _cloudinary;

    public BlogService(IBaseRepository<Blog, Guid> blogRepository, IBaseRepository<Image, Guid> imageRepository, CloudinaryLogic cloudinary, IBaseRepository<Trip, Guid> tripRepository)
    {
        _blogRepository = blogRepository;
        _imageRepository = imageRepository;
        _cloudinary = cloudinary;
        _tripRepository = tripRepository;
    }

    /// <summary>
    /// Select blog by requestBlogId
    /// </summary>
    /// <param name="requestBlogId"></param>
    /// <returns></returns>
    public async Task<Ecq100SelectBlogResponse> SelectBlog(Guid requestBlogId)
    {
        var response = new Ecq100SelectBlogResponse { Success = false };

        // Select blog
        var blogSelect = await _blogRepository.GetView<VwBlog>(x => x.BlogId == requestBlogId).FirstOrDefaultAsync()!;
        if (blogSelect == null)
        {
            response.SetMessage(MessageId.E00000, CommonMessages.BlogNotFound);
            return response;
        }
        
        // Select blog images
        var blogImageUrls = await _imageRepository
            .GetView<VwImage>(x => x.EntityId == requestBlogId &&
                                   x.EntityType == ConstantEnum.EntityType.Blog.ToString())
            .Select(x => x.ImageUrl)
            .ToListAsync();
        
        response.Response = new Ecq100SelectBlogEntity
        {
            BlogId = blogSelect.BlogId,
            Title = blogSelect.Title,
            Content = blogSelect.Content,
            CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(blogSelect.CreatedAt),
            UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(blogSelect.UpdatedAt),
            AuthorId = blogSelect.AuthorId,
            AuthorFirstName = blogSelect.AuthorFirstName,
            AuthorLastName = blogSelect.AuthorLastName,
            DestinationId = (blogSelect.DestinationId.ToString() ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => Guid.TryParse(id.Trim(), out var guid) ? guid : Guid.Empty)
                .Where(id => id != Guid.Empty)
                .ToList(),
            
            DestinationName = (blogSelect.DestinationName ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(name => name.Trim())
                .ToList(),
            AuthorAvatar = blogSelect.AuthorAvatar,
            BlogImages = blogImageUrls!,
        };
        
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
        var response = new Ecq100SelectBlogsResponse { Success = false };
        var blogResponses = new List<Ecq100SelectBlogsEntity>();

        // Select blogs
        var blogSelects = await _blogRepository.GetView<VwBlog>().ToListAsync();
        foreach (var blog in blogSelects)
        {
            var blogResponse = new Ecq100SelectBlogsEntity
            {
                BlogId = blog.BlogId,
                Title = blog.Title,
                Content = blog.Content,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(blog.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(blog.UpdatedAt),
                AuthorId = blog.AuthorId,
                AuthorFirstName = blog.AuthorFirstName,
                AuthorLastName = blog.AuthorLastName,
                DestinationId = (blog.DestinationId ?? "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => Guid.TryParse(id.Trim(), out var guid) ? guid : Guid.Empty)
                    .Where(id => id != Guid.Empty)
                    .ToList(),
                
                DestinationName = (blog.DestinationName ?? "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(name => name.Trim())
                    .ToList()
            };

            blogResponses.Add(blogResponse);
        }
        
        // Select blog images
        var blogIds = blogResponses.Select(b => b.BlogId).ToList();

        var allImages = await _imageRepository
            .GetView<VwImage>(x => blogIds.Contains(x.EntityId) && x.EntityType == ConstantEnum.EntityType.Blog.ToString())
            .ToListAsync();

        var imagesByBlogId = allImages
            .GroupBy(img => img.EntityId)
            .ToDictionary(g => g.Key, g => g.Select(img => img.ImageUrl).ToList());

        foreach (var blog in blogResponses)
        {
            blog.BlogImages = imagesByBlogId.TryGetValue(blog.BlogId, out var urls)
                ? urls
                : new List<string>();
        }

        // True
        response.Success = true;
        response.Response = blogResponses;
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

        // Check trip status
        var tripIsCompleted = await _tripRepository.Find(x => x.TripId == request.TripId && x.IsActive == true)
            .FirstOrDefaultAsync();
        if (tripIsCompleted == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.TripNotFound);
            return response;
        }

        // Check if trip is completed
        if (tripIsCompleted.Status != (byte)ConstantEnum.TripStatus.Completed)
        {
            response.SetMessage(MessageId.I00000, "You can only create a blog for a completed trip.");
            return response;
        }

        // Verify ownership
        if (tripIsCompleted.UserId != Guid.Parse(identityEntity.UserId))
        {
            response.SetMessage(MessageId.I00000, CommonMessages.NotAuthorizedToManageTrip);
            return response;
        }

        // Check if blog already exists for the trip
        var existingBlog = await _blogRepository.Find(x => x.TripId == request.TripId && x.IsActive == true).FirstOrDefaultAsync();
        if (existingBlog != null)
        {
            response.SetMessage(MessageId.I00000, "You have already created a blog for this trip.");
            return response;
        }

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
                TripId = request.TripId,
            };
            // Add blog to repository
            await _blogRepository.AddAsync(blog);

            // Upload images if provided
            foreach (var image in request.BlogImages)
            {
                // Upload image to Cloudinary and get URL
                var imageUrl = await _cloudinary.UploadImageAsync(image);

                // Create new image entity
                var blogImage = new Image
                {
                    EntityId = blog.BlogId,
                    ImageUrl = imageUrl,
                    EntityType = ConstantEnum.EntityType.Blog.ToString(),
                };
                // Add image to repository
                await _imageRepository.AddAsync(blogImage);
            }
            // Save images
            await _imageRepository.SaveChangesAsync(identityEntity.Email);

            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }
}