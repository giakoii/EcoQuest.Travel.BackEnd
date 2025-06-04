using BackEnd.DTOs.Ecq110;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq110;

/// <summary>
/// Ecq110SelectTripController - Select a trip by ID
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq110SelectTripController : AbstractApiAsyncControllerNotToken<Ecq110SelectTripRequest, Ecq110SelectTripResponse, Ecq110TripEntity>
{
    private readonly ITripService _tripService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tripService"></param>
    public Ecq110SelectTripController(ITripService tripService)
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
    public override async Task<Ecq110SelectTripResponse> ProcessRequest([FromQuery] Ecq110SelectTripRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq110SelectTripResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq110SelectTripResponse> Exec(Ecq110SelectTripRequest request)
    {
        return await _tripService.SelectTrip(request.TripId);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq110SelectTripResponse ErrorCheck(Ecq110SelectTripRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq110SelectTripResponse { Success = false };
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
