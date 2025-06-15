using BackEnd.DTOs.Ecq230;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq100;

/// <summary>
/// Ecq230SelectAttractionsController - Select attractions
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq100SelectAttractionsController : AbstractApiAsyncControllerNotToken<Ecq230SelectAttractionsRequest, Ecq230SelectAttractionsResponse, List<Ecq230AttractionEntity>>
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
    public override async Task<Ecq230SelectAttractionsResponse> ProcessRequest([FromQuery] Ecq230SelectAttractionsRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq230SelectAttractionsResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq230SelectAttractionsResponse> Exec(Ecq230SelectAttractionsRequest request)
    {
        return await _attractionService.Ecq100SelectAttractions();
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq230SelectAttractionsResponse ErrorCheck(Ecq230SelectAttractionsRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq230SelectAttractionsResponse() { Success = false };
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
