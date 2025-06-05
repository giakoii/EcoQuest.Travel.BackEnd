using BackEnd.DTOs.Ecq110;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq110;

/// <summary>
/// Ecq110SelectTripsController - Select all trips
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq110SelectTripsController : AbstractApiAsyncControllerNotToken<Ecq110SelectTripsRequest, Ecq110SelectTripsResponse, List<Ecq110TripListEntity>>
{
    private readonly ITripService _tripService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tripService"></param>
    public Ecq110SelectTripsController(ITripService tripService)
    {
        _tripService = tripService;
    }

    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq110SelectTripsResponse> ProcessRequest(Ecq110SelectTripsRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq110SelectTripsResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq110SelectTripsResponse> Exec([FromQuery] Ecq110SelectTripsRequest request)
    {
        return await _tripService.SelectTrips();
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq110SelectTripsResponse ErrorCheck(Ecq110SelectTripsRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq110SelectTripsResponse { Success = false };
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
