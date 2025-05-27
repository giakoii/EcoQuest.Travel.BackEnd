using BackEnd.DTOs.Trip;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Trip;

/// <summary>
/// SelectTripController - Select the trip of the user
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
public class SelectTripController : AbstractApiGetController<SelectTripRequest, SelectTripResponse, SelectTripEntity>
{
    private readonly ITripService _tripService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tripService"></param>
    /// <param name="identityApiClient"></param>
    public SelectTripController(ITripService tripService, IIdentityApiClient identityApiClient)
    {
        _tripService = tripService;
        _identityApiClient = identityApiClient;
    }
    
    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override SelectTripResponse Get(SelectTripRequest request)
    {
        return Get(request, _logger, new SelectTripResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override SelectTripResponse Exec(SelectTripRequest request)
    {
        return _tripService.SelectTrip(request, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override SelectTripResponse ErrorCheck(SelectTripRequest request, List<DetailError> detailErrorList)
    {
        var response = new SelectTripResponse() { Success = false };
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