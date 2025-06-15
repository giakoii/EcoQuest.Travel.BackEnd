using BackEnd.DTOs.Ecq200;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using OpenIddict.Validation.AspNetCore;

namespace BackEnd.Controllers.V1.Ecq200;

/// <summary>
/// Ecq200UpdateDestinationController - Update destination
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq200UpdateDestinationController : AbstractApiAsyncController<Ecq200UpdateDestinationRequest, Ecq200UpdateDestinationResponse, string>
{
    private readonly IDestinationService _destinationService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="destinationService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq200UpdateDestinationController(IDestinationService destinationService, IIdentityApiClient identityApiClient)
    {
        _destinationService = destinationService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Put
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    public override async Task<Ecq200UpdateDestinationResponse> ProcessRequest(Ecq200UpdateDestinationRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq200UpdateDestinationResponse());
    }
    
    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq200UpdateDestinationResponse> Exec(Ecq200UpdateDestinationRequest request)
    {
        return await _destinationService.UpdateDestination(request);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq200UpdateDestinationResponse ErrorCheck(Ecq200UpdateDestinationRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq200UpdateDestinationResponse() { Success = false };
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
