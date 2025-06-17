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
/// Ecq110SelectTripScheduleController - Select trip schedule by ID
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq110SelectTripScheduleController : AbstractApiAsyncController<Ecq110SelectTripScheduleRequest, Ecq110SelectTripScheduleResponse, List<Ecq110TripScheduleEntity>>
{
    private readonly ITripService _tripService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tripService">Trip service</param>
    /// <param name="identityApiClient"></param>
    public Ecq110SelectTripScheduleController(ITripService tripService, IIdentityApiClient identityApiClient)
    {
        _tripService = tripService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request">Request info</param>
    /// <returns>Response info</returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq110SelectTripScheduleResponse> ProcessRequest([FromQuery] Ecq110SelectTripScheduleRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq110SelectTripScheduleResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request">Request info</param>
    /// <returns>Response info</returns>
    protected override async Task<Ecq110SelectTripScheduleResponse> Exec(Ecq110SelectTripScheduleRequest request)
    {
        return await _tripService.SelectTripSchedule(request.ScheduleId);
    }

    /// <summary>
    ///Error check
    /// </summary>
    /// <param name="request">Request object</param>
    /// <param name="detailErrorList">Detail error list</param>
    /// <returns>Response object</returns>
    protected internal override Ecq110SelectTripScheduleResponse ErrorCheck(Ecq110SelectTripScheduleRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq110SelectTripScheduleResponse() { Success = false };
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
