using BackEnd.DTOs.Ecq110;
using BackEnd.Logics;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq110;

/// <summary>
/// Ecq110InsertTripScheduleWithAi - Handles chatbot requests
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq110InsertTripScheduleWithAiController : AbstractApiAsyncController<Ecq110InsertTripScheduleWithAiRequest, Ecq110InsertTripScheduleWithAiResponse, List<Ecq110InsertTripScheduleResponseDetail>>
{
    private readonly ITripScheduleService _tripScheduleService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tripScheduleService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq110InsertTripScheduleWithAiController(ITripScheduleService tripScheduleService, IIdentityApiClient identityApiClient)
    {
        _tripScheduleService = tripScheduleService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Post
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq110InsertTripScheduleWithAiResponse> ProcessRequest(Ecq110InsertTripScheduleWithAiRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq110InsertTripScheduleWithAiResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq110InsertTripScheduleWithAiResponse> Exec(Ecq110InsertTripScheduleWithAiRequest request)
    {
        return await _tripScheduleService.InsertTripScheduleUseAi(request, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq110InsertTripScheduleWithAiResponse ErrorCheck(Ecq110InsertTripScheduleWithAiRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq110InsertTripScheduleWithAiResponse() { Success = false };
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