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
/// Ecq110SelectTripSchedulesController - Select all trip schedules for a specific trip
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq110SelectTripSchedulesController : AbstractApiAsyncController<Ecq110SelectTripSchedulesRequest, Ecq110SelectTripSchedulesResponse, List<Ecq110TripScheduleEntity>>
{
    private readonly ITripService _tripService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tripService">Trip service</param>
    public Ecq110SelectTripSchedulesController(ITripService tripService, IIdentityApiClient identityApiClient)
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
    public override async Task<Ecq110SelectTripSchedulesResponse> ProcessRequest([FromQuery] Ecq110SelectTripSchedulesRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq110SelectTripSchedulesResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request">Request info</param>
    /// <returns>Response info</returns>
    protected override async Task<Ecq110SelectTripSchedulesResponse> Exec(Ecq110SelectTripSchedulesRequest request)
    {
        return await _tripService.SelectTripSchedules(request.TripId);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request">Request object</param>
    /// <param name="detailErrorList">Detail error list</param>
    /// <returns>Response object</returns>
    protected internal override Ecq110SelectTripSchedulesResponse ErrorCheck(Ecq110SelectTripSchedulesRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq110SelectTripSchedulesResponse() { Success = false };
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
