using BackEnd.DTOs.Ecq100;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq100;

/// <summary>
/// Ecq100SelectBlogController - Select blog
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq100SelectBlogsController : AbstractApiAsyncControllerNotToken<Ecq100SelectBlogsRequest, Ecq100SelectBlogsResponse, List<Ecq100SelectBlogsEntity>>
{
    private readonly IBlogService _blogService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="blogService"></param>
    public Ecq100SelectBlogsController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public override async Task<Ecq100SelectBlogsResponse> ProcessRequest(Ecq100SelectBlogsRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq100SelectBlogsResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq100SelectBlogsResponse> Exec(Ecq100SelectBlogsRequest request)
    {
        return await _blogService.SelectBlogs();
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq100SelectBlogsResponse ErrorCheck(Ecq100SelectBlogsRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq100SelectBlogsResponse() { Success = false };
        if (detailErrorList.Count > 0)
        {
            // Error
            response.SetMessage(MessageId.E10000);
            response.DetailErrorList = detailErrorList;
            return response;
        }
        // True
        response.Success = true;
        return response;
    }
}