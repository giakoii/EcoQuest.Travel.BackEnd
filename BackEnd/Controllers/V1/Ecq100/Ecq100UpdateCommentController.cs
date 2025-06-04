using BackEnd.DTOs.Ecq100;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq100;

/// <summary>
/// Ecq100UpdateCommentController - Update comment
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq100UpdateCommentController : AbstractApiAsyncController<Ecq100UpdateCommentRequest, Ecq100UpdateCommentResponse, string>
{
    private readonly ICommentService _commentService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commentService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq100UpdateCommentController(ICommentService commentService, IIdentityApiClient identityApiClient)
    {
        _commentService = commentService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Put
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq100UpdateCommentResponse> ProcessRequest(Ecq100UpdateCommentRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq100UpdateCommentResponse());
    }
    
    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq100UpdateCommentResponse> Exec(Ecq100UpdateCommentRequest request)
    {
        return await _commentService.UpdateComment(request, _identityEntity);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq100UpdateCommentResponse ErrorCheck(Ecq100UpdateCommentRequest request,
        List<DetailError> detailErrorList)
    {
        var response = new Ecq100UpdateCommentResponse() { Success = false };
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