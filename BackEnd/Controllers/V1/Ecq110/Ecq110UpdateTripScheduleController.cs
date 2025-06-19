using BackEnd.DTOs.Ecq110;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using OpenIddict.Validation.AspNetCore;

namespace BackEnd.Controllers.V1.Ecq110;

/// <summary>
/// Ecq110UpdateTripScheduleController - Update a trip schedule
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class Ecq110UpdateTripScheduleController : AbstractApiAsyncController<Ecq110UpdateTripScheduleRequest, Ecq110UpdateTripScheduleResponse, bool>
{
    private readonly ITripScheduleService _tripService;
    private readonly Logger _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tripService">Trip service</param>
    /// <param name="identityApiClient">Identity API client</param>
    public Ecq110UpdateTripScheduleController(ITripScheduleService tripService, IIdentityApiClient identityApiClient)
    {
        _tripService = tripService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Put 
    /// </summary>
    /// <param name="request">Request info</param>
    /// <returns>Response info</returns>
    [HttpPut]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq110UpdateTripScheduleResponse> ProcessRequest([FromBody] Ecq110UpdateTripScheduleRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq110UpdateTripScheduleResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request">Request info</param>
    /// <returns>Response info</returns>
    protected override async Task<Ecq110UpdateTripScheduleResponse> Exec(Ecq110UpdateTripScheduleRequest request)
    {
        return await _tripService.UpdateTripSchedule(request, _identityEntity);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request">Request object</param>
    /// <param name="detailErrorList">Detail error list</param>
    /// <returns>Response object</returns>
    protected internal override Ecq110UpdateTripScheduleResponse ErrorCheck(Ecq110UpdateTripScheduleRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq110UpdateTripScheduleResponse() { Success = false };
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
