using BackEnd.DTOs.Ecq110;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq110;

/// <summary>
/// Ecq110SelectServiceController - Select service for trip schedule
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq110SelectServiceController : AbstractApiAsyncController<Ecq110SelectServiceRequest, Ecq110SelectServiceResponse, List<Ecq110SelectServiceEntity>>
{
    private readonly ITripScheduleService _tripScheduleService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tripScheduleService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq110SelectServiceController(ITripScheduleService tripScheduleService, IIdentityApiClient identityApiClient)
    {
        _tripScheduleService = tripScheduleService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq110SelectServiceResponse> ProcessRequest([FromQuery] Ecq110SelectServiceRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq110SelectServiceResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq110SelectServiceResponse> Exec(Ecq110SelectServiceRequest request)
    {
        return await _tripScheduleService.SelectService(request);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq110SelectServiceResponse ErrorCheck(Ecq110SelectServiceRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq110SelectServiceResponse() { Success = false };
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