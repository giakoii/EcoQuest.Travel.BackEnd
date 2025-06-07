using BackEnd.DTOs.Ecq110;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq110;

/// <summary>
/// Ecq110InsertTripScheduleController - Insert a new trip schedule
/// </summary>
//[ApiController]
//[Route("api/v1/[controller]")]
public class Ecq110InsertTripScheduleController : AbstractApiAsyncController<Ecq110InsertTripScheduleRequest, Ecq110InsertTripScheduleResponse, string>
{
    private readonly ITripService _tripService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tripService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq110InsertTripScheduleController(ITripService tripService, IIdentityApiClient identityApiClient)
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
    public override async Task<Ecq110InsertTripScheduleResponse> ProcessRequest(Ecq110InsertTripScheduleRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq110InsertTripScheduleResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq110InsertTripScheduleResponse> Exec(Ecq110InsertTripScheduleRequest request)
    {
        return await _tripService.InsertTripSchedule(request, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq110InsertTripScheduleResponse ErrorCheck(Ecq110InsertTripScheduleRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq110InsertTripScheduleResponse { Success = false };
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
