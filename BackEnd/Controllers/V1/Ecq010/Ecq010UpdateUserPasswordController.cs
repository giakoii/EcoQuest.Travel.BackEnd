using BackEnd.DTOs.User;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;

namespace BackEnd.Controllers.V1.Ecq010;

/// <summary>
/// Ecq010UpdateUserPasswordController - Update user password
/// </summary>
[Route("/api/v1/[controller]")]
[ApiController]
public class Ecq010UpdateUserPasswordController : AbstractApiAsyncController<Ecq010UpdateUserPasswordRequest, Ecq010UpdateUserPasswordResponse, string>
{
    private readonly IUserService _userService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq010UpdateUserPasswordController(IUserService userService, IIdentityApiClient identityApiClient)
    {
        _userService = userService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Patch
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq010UpdateUserPasswordResponse> ProcessRequest(Ecq010UpdateUserPasswordRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq010UpdateUserPasswordResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq010UpdateUserPasswordResponse> Exec(Ecq010UpdateUserPasswordRequest request)
    {
        return await _userService.UpdateUserPassword(request, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq010UpdateUserPasswordResponse ErrorCheck(Ecq010UpdateUserPasswordRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq010UpdateUserPasswordResponse() { Success = false };
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

