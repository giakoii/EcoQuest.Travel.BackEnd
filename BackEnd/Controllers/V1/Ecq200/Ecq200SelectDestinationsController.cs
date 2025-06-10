using BackEnd.DTOs.Ecq200;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq200;

/// <summary>
/// Ecq200SelectDestinationsController - Select all destinations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq200SelectDestinationsController : AbstractApiAsyncControllerNotToken<Ecq200SelectDestinationsRequest, Ecq200SelectDestinationsResponse, List<Ecq200DestinationEntity>>
{
    private readonly IDestinationService _destinationService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="destinationService">Destination service</param>
    public Ecq200SelectDestinationsController(IDestinationService destinationService)
    {
        _destinationService = destinationService;
    }

    /// <summary>
    /// Process API request
    /// </summary>
    /// <param name="request">Request info</param>
    /// <returns>Response info</returns>
    [HttpGet]
    public override async Task<Ecq200SelectDestinationsResponse> ProcessRequest([FromQuery] Ecq200SelectDestinationsRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq200SelectDestinationsResponse());
    }

    /// <summary>
    /// Execute business logic
    /// </summary>
    /// <param name="request">Request info</param>
    /// <returns>Response info</returns>
    protected override async Task<Ecq200SelectDestinationsResponse> Exec(Ecq200SelectDestinationsRequest request)
    {
        return await _destinationService.SelectDestinations();
    }

    /// <summary>
    /// Check error
    /// </summary>
    /// <param name="request">Request object</param>
    /// <param name="detailErrorList">Detail error list</param>
    /// <returns>Response object</returns>
    protected internal override Ecq200SelectDestinationsResponse ErrorCheck(Ecq200SelectDestinationsRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq200SelectDestinationsResponse() { Success = false };
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
