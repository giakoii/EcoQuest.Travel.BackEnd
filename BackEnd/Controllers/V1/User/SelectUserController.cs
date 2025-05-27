using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using OpenIddict.Validation.AspNetCore;

namespace BackEnd.Controllers.V1.User;

/// <summary>
/// SelectProfileController - Select User Profile
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class SelectUserController : AbstractApiGetAsyncController<SelectUserRequest, SelectUserResponse, SelectUserEntity>
{
    private readonly IUserService _userService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userService"></param>
    /// <param name="identityApiClient"></param>
    public SelectUserController(IUserService userService, IIdentityApiClient identityApiClient)
    {
        _userService = userService;
        _identityApiClient = identityApiClient;
    }
    
    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<SelectUserResponse> Get(SelectUserRequest request)
    {
        return await Get(request, _logger, new SelectUserResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<SelectUserResponse> Exec(SelectUserRequest request)
    {
        return await _userService.SelectUser(request, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override SelectUserResponse ErrorCheck(SelectUserRequest request, List<DetailError> detailErrorList)
    {
        var response = new SelectUserResponse() { Success = false };
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