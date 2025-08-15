using BackEnd.DTOs.Ecq100;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq100;

/// <summary>
/// Ecq100SelectAttractionsController - Select attractions
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq100SelectAttractionsController : AbstractApiAsyncControllerNotToken<Ecq100SelectAttractionsRequest, Ecq100SelectAttractionsResponse, List<Ecq100SelectAttractionsEntity>>
{
    private readonly IAttractionService _attractionService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attractionService"></param>
    public Ecq100SelectAttractionsController(IAttractionService attractionService)
    {
        _attractionService = attractionService;
    }

    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public override async Task<Ecq100SelectAttractionsResponse> ProcessRequest([FromQuery] Ecq100SelectAttractionsRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq100SelectAttractionsResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq100SelectAttractionsResponse> Exec(Ecq100SelectAttractionsRequest request)
    {
        return await _attractionService.Ecq100SelectAttractions(request);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq100SelectAttractionsResponse ErrorCheck(Ecq100SelectAttractionsRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq100SelectAttractionsResponse() { Success = false };
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
