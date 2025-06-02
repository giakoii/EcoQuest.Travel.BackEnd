using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using OpenIddict.Validation.AspNetCore;

namespace BackEnd.Controllers.V1.User;

/// <summary>
/// Ecq300UpdateUserController - Update Ecq300 Information
/// </summary>
[Route("api/v1/user/[controller]")]
[ApiController]
public class Ecq300UpdateUserController : AbstractApiAsyncController<Ecq300UpdateUserRequest, Ecq300UpdateUserResponse, string>
{
    private readonly IUserService _userService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq300UpdateUserController(IUserService userService, IIdentityApiClient identityApiClient)
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
    public override async Task<Ecq300UpdateUserResponse> ProcessRequest([FromForm] Ecq300UpdateUserRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq300UpdateUserResponse());
    }

    protected override async Task<Ecq300UpdateUserResponse> Exec(Ecq300UpdateUserRequest request)
    {
        return await _userService.UpdateUser(request, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq300UpdateUserResponse ErrorCheck(Ecq300UpdateUserRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq300UpdateUserResponse() { Success = false };
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