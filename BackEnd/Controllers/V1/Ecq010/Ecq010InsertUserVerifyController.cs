using BackEnd.Controllers.V1.User;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq010;

/// <summary>
/// Ecq010InsertUserVerifyController - Verify the user registration
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
public class Ecq010InsertUserVerifyController : AbstractApiAsyncControllerNotToken<Ecq010InsertUserVerifyRequest, Ecq010InsertUserVerifyResponse, string>
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IUserService _userService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userService"></param>
    public Ecq010InsertUserVerifyController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Incoming Post
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]    
    public override Task<Ecq010InsertUserVerifyResponse> ProcessRequest(Ecq010InsertUserVerifyRequest request)
    {
        return ProcessRequest(request, _logger, new Ecq010InsertUserVerifyResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override Task<Ecq010InsertUserVerifyResponse> Exec(Ecq010InsertUserVerifyRequest request)
    {
        return _userService.InsertUserVerify(request);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq010InsertUserVerifyResponse ErrorCheck(Ecq010InsertUserVerifyRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq010InsertUserVerifyResponse() { Success = false };
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