using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using OpenIddict.Validation.AspNetCore;

namespace BackEnd.Controllers.V1.User;

/// <summary>
/// UpdateUserController - Update User Information
/// </summary>
[Route("api/v1/user/[controller]")]
[ApiController]
public class UpdateUserController : AbstractApiAsyncController<UpdateUserRequest, UpdateUserResponse, string>
{
    private readonly IUserService _userService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userService"></param>
    /// <param name="identityApiClient"></param>
    public UpdateUserController(IUserService userService, IIdentityApiClient identityApiClient)
    {
        _userService = userService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Put
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<UpdateUserResponse> ProcessRequest([FromForm] UpdateUserRequest request)
    {
        return await ProcessRequest(request, _logger, new UpdateUserResponse());
    }

    protected override async Task<UpdateUserResponse> Exec(UpdateUserRequest request)
    {
        return await _userService.UpdateUser(request, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override UpdateUserResponse ErrorCheck(UpdateUserRequest request, List<DetailError> detailErrorList)
    {
        var response = new UpdateUserResponse() { Success = false };
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