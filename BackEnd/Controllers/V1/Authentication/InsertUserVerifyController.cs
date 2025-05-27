using BackEnd.Controllers;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.User;

/// <summary>
/// InsertUserVerifyController - Verify the user registration
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
public class InsertUserVerifyController : AbstractApiAsyncControllerNotToken<InsertUserVerifyRequest, InsertUserVerifyResponse, string>
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IUserService _userService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userService"></param>
    public InsertUserVerifyController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Incoming Post
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]    
    public override Task<InsertUserVerifyResponse> ProcessRequest(InsertUserVerifyRequest request)
    {
        return ProcessRequest(request, _logger, new InsertUserVerifyResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override Task<InsertUserVerifyResponse> Exec(InsertUserVerifyRequest request)
    {
        return _userService.InsertUserVerify(request);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override InsertUserVerifyResponse ErrorCheck(InsertUserVerifyRequest request, List<DetailError> detailErrorList)
    {
        var response = new InsertUserVerifyResponse() { Success = false };
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