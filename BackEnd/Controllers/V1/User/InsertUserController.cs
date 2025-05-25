using BackEnd.Controllers;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.User;

/// <summary>
/// InsertUserController - Insert new user
/// </summary>
[Route("/api/v1/[controller]")]
[ApiController]
public class InsertUserController : AbstractApiAsyncControllerNotToken<InsertUserRequest, InsertUserResponse, string>
{
    private readonly IUserService _userService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userService"></param>
    public InsertUserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Incoming Post
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public override async Task<InsertUserResponse> ProcessRequest(InsertUserRequest request)
    {
        return await ProcessRequest(request, _logger, new InsertUserResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<InsertUserResponse> Exec(InsertUserRequest request)
    {
        return await _userService.InsertUser(request);
    }
    
    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override InsertUserResponse ErrorCheck(InsertUserRequest request, List<DetailError> detailErrorList)
    {
        var response = new InsertUserResponse() { Success = false };
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