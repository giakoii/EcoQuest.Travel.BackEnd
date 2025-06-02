using BackEnd.DTOs.Ecq200;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq200;

/// <summary>
/// Ecq200InsertDestinationController - Insert destination
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq200InsertDestinationController : AbstractApiAsyncController<Ecq200InsertDestinationRequest, Ecq200InsertDestinationResponse, string>
{
    private readonly IDestinationService _destinationService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="destinationService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq200InsertDestinationController(IDestinationService destinationService, IIdentityApiClient identityApiClient)
    {
        _destinationService = destinationService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming POST
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public override async Task<Ecq200InsertDestinationResponse> ProcessRequest([FromForm]Ecq200InsertDestinationRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq200InsertDestinationResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq200InsertDestinationResponse> Exec(Ecq200InsertDestinationRequest request)
    {
        return await _destinationService.InsertDestination(request);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq200InsertDestinationResponse ErrorCheck(Ecq200InsertDestinationRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq200InsertDestinationResponse() { Success = false };
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