using BackEnd.DTOs.Ecq100;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq100;

/// <summary>
/// Ecq100DeleteCommentController - Delete comment
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq100DeleteCommentController : AbstractApiAsyncController<Ecq100DeleteCommentRequest, Ecq100DeleteCommentResponse, string>
{
   private readonly ICommentService _commentService;
   private readonly Logger _logger = LogManager.GetCurrentClassLogger();

   /// <summary>
   /// Constructor
   /// </summary>
   /// <param name="commentService"></param>
   /// <param name="identityApiClient"></param>
   public Ecq100DeleteCommentController(ICommentService commentService, IIdentityApiClient identityApiClient)
   {
       _commentService = commentService;
       _identityApiClient = identityApiClient;
   }

   /// <summary>
   /// Incoming Delete
   /// </summary>
   /// <param name="request"></param>
   /// <returns></returns>
   [HttpPatch]
   [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
   public override async Task<Ecq100DeleteCommentResponse> ProcessRequest(Ecq100DeleteCommentRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq100DeleteCommentResponse());
    }

   /// <summary>
   /// Main processing
   /// </summary>
   /// <param name="request"></param>
   /// <returns></returns>
    protected override async Task<Ecq100DeleteCommentResponse> Exec(Ecq100DeleteCommentRequest request)
    {
        return await _commentService.DeleteComment(request, _identityEntity);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq100DeleteCommentResponse ErrorCheck(Ecq100DeleteCommentRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq100DeleteCommentResponse() { Success = false };
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
