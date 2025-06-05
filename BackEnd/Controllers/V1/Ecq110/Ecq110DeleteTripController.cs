using BackEnd.DTOs.Ecq110;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq110;

/// <summary>
/// Ecq110DeleteTripController - Delete trip
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq110DeleteTripController : AbstractApiAsyncController<Ecq110DeleteTripRequest, Ecq110DeleteTripResponse, string>
{
    private readonly ITripService _tripService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tripService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq110DeleteTripController(ITripService tripService, IIdentityApiClient identityApiClient)
    {
        _tripService = tripService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Delete
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq110DeleteTripResponse> ProcessRequest(Ecq110DeleteTripRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq110DeleteTripResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq110DeleteTripResponse> Exec(Ecq110DeleteTripRequest request)
    {
        return await _tripService.DeleteTrip(request, _identityEntity);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq110DeleteTripResponse ErrorCheck(Ecq110DeleteTripRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq110DeleteTripResponse { Success = false };
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
