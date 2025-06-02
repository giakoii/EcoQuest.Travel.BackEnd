using BackEnd.DTOs.Trip;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Trip;

/// <summary>
/// InsertTripController - Creates a new trip
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
public class InsertTripController : AbstractApiAsyncController<InsertTripRequest, InsertTripResponse, string>
{
    private readonly ITripService _tripService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tripService"></param>
    /// <param name="identityApiClient"></param>
    public InsertTripController(ITripService tripService, IIdentityApiClient identityApiClient)
    {
        _tripService = tripService;
        _identityApiClient = identityApiClient;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<InsertTripResponse> ProcessRequest(InsertTripRequest request)
    {
        return await ProcessRequest(request, _logger, new InsertTripResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<InsertTripResponse> Exec(InsertTripRequest request)
    {
        return await _tripService.InsertTrip(request, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override InsertTripResponse ErrorCheck(InsertTripRequest request, List<DetailError> detailErrorList)
    {
        var response = new InsertTripResponse() { Success = false };
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