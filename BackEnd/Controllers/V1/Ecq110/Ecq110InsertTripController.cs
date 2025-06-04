using BackEnd.DTOs.Ecq110;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq110;

/// <summary>
/// Ecq110InsertTripController - Insert a new trip
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq110InsertTripController : AbstractApiAsyncController<Ecq110InsertTripRequest, Ecq110InsertTripResponse, string>
{
    private readonly ITripService _tripService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tripService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq110InsertTripController(ITripService tripService, IIdentityApiClient identityApiClient)
    {
        _tripService = tripService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Post
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq110InsertTripResponse> ProcessRequest(Ecq110InsertTripRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq110InsertTripResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq110InsertTripResponse> Exec(Ecq110InsertTripRequest request)
    {
        return await _tripService.InsertTrip(request, _identityEntity);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq110InsertTripResponse ErrorCheck(Ecq110InsertTripRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq110InsertTripResponse { Success = false };
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
