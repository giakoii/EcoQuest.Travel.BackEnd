using BackEnd.Controllers.V1.User;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using OpenIddict.Validation.AspNetCore;

namespace BackEnd.Controllers.V1.Ecq300;

/// <summary>
/// SelectProfileController - Select Ecq300 Profile
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq300SelectUserController : AbstractApiGetAsyncController<Ecq300SelectUserRequest, Ecq300SelectUserResponse, Ecq300SelectUserEntity>
{
    private readonly IUserService _userService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq300SelectUserController(IUserService userService, IIdentityApiClient identityApiClient)
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
    public override async Task<Ecq300SelectUserResponse> Get(Ecq300SelectUserRequest request)
    {
        return await Get(request, _logger, new Ecq300SelectUserResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq300SelectUserResponse> Exec(Ecq300SelectUserRequest request)
    {
        return await _userService.SelectUser(request, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq300SelectUserResponse ErrorCheck(Ecq300SelectUserRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq300SelectUserResponse() { Success = false };
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