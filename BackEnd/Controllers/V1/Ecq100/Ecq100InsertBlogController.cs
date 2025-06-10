using BackEnd.DTOs.Ecq100;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq100;

/// <summary>
/// Ecq100InsertBlogController - Insert blog
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq100InsertBlogController : AbstractApiAsyncController<Ecq100InsertBlogRequest, Ecq100InsertBlogResponse, string>
{
    private readonly IBlogService _blogService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="blogService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq100InsertBlogController(IBlogService blogService, IIdentityApiClient identityApiClient)
    {
        _blogService = blogService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Post
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq100InsertBlogResponse> ProcessRequest([FromForm] Ecq100InsertBlogRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq100InsertBlogResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq100InsertBlogResponse> Exec(Ecq100InsertBlogRequest request)
    {
        return await _blogService.InsertBlog(request, _identityEntity);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq100InsertBlogResponse ErrorCheck(Ecq100InsertBlogRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq100InsertBlogResponse() { Success = false };
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