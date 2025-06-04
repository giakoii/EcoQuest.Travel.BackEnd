using BackEnd.DTOs.Ecq100;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq100;

/// <summary>
/// Ecq100SelectCommentsController - Select comments
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq100SelectCommentsController : AbstractApiAsyncControllerNotToken<Ecq100SelectCommentsRequest, Ecq100SelectCommentsResponse, List<Ecq100SelectCommentsEntity>>
{
    private readonly ICommentService _commentService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    public Ecq100SelectCommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }
    
    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public override async Task<Ecq100SelectCommentsResponse> ProcessRequest(Ecq100SelectCommentsRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq100SelectCommentsResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq100SelectCommentsResponse> Exec(Ecq100SelectCommentsRequest request)
    {
        return await _commentService.SelectComments(request.BlogId);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq100SelectCommentsResponse ErrorCheck(Ecq100SelectCommentsRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq100SelectCommentsResponse() { Success = false };
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