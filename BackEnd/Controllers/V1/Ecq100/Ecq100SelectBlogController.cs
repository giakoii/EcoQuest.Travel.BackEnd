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
public class Ecq100SelectBlogController : AbstractApiAsyncControllerNotToken<Ecq100SelectBlogRequest, Ecq100SelectBlogResponse, Ecq100SelectBlogEntity>
{
    private readonly IBlogService _blogService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="blogService"></param>
    public Ecq100SelectBlogController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public override async Task<Ecq100SelectBlogResponse> ProcessRequest([FromQuery] Ecq100SelectBlogRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq100SelectBlogResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq100SelectBlogResponse> Exec(Ecq100SelectBlogRequest request)
    {
        return await _blogService.SelectBlog(request.BlogId);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq100SelectBlogResponse ErrorCheck(Ecq100SelectBlogRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq100SelectBlogResponse() { Success = false };
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