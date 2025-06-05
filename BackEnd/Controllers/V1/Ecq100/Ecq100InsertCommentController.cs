using BackEnd.DTOs.Ecq100;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq100;

/// <summary>
/// Ecq100InsertCommentController - Insert comment
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq100InsertCommentController : AbstractApiAsyncController<Ecq100InsertCommentRequest, Ecq100InsertCommentResponse, string>
{
    private readonly ICommentService _commentService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commentService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq100InsertCommentController(ICommentService commentService, IIdentityApiClient identityApiClient)
    {
        _commentService = commentService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Post
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq100InsertCommentResponse> ProcessRequest(Ecq100InsertCommentRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq100InsertCommentResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq100InsertCommentResponse> Exec(Ecq100InsertCommentRequest request)
    {
        return await _commentService.InsertComment(request, _identityEntity);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq100InsertCommentResponse ErrorCheck(Ecq100InsertCommentRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq100InsertCommentResponse() { Success = false };
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