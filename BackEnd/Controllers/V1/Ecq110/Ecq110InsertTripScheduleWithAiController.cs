using BackEnd.DTOs.Chatbot;
using BackEnd.Logics;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq110;

/// <summary>
/// Ecq110InsertTripScheduleWithAi - Handles chatbot requests
/// </summary>
// [ApiController]
// [Route("api/v1/[controller]")]
public class Ecq110InsertTripScheduleWithAiController : AbstractApiAsyncController<Ecq110InsertTripScheduleWithAiRequest, Ecq110InsertTripScheduleWithAiResponse, List<Ecq110InsertTripScheduleWithAiEntity>>
{
    private readonly ITripService _tripService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tripService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq110InsertTripScheduleWithAiController(ITripService tripService, IIdentityApiClient identityApiClient)
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
        var chatbotResponse = new Ecq110InsertTripScheduleWithAiResponse { Success = false };
        
        
        chatbotResponse.Success = true;
        chatbotResponse.SetMessage(MessageId.I00000);
        return chatbotResponse;
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