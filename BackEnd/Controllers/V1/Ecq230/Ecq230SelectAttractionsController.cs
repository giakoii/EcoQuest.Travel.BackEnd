using BackEnd.DTOs.Ecq230;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq230;

/// <summary>
/// Ecq230SelectAttractionsController - Select attractions
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq230SelectAttractionsController : AbstractApiAsyncController<Ecq230SelectAttractionsRequest, Ecq230SelectAttractionsResponse, List<Ecq230AttractionEntity>>
{
    private readonly IAttractionService _attractionService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attractionService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq230SelectAttractionsController(IAttractionService attractionService, IIdentityApiClient identityApiClient)
    {
        _attractionService = attractionService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = ConstRole.Partner, AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
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
        return await _attractionService.Ecq230SelectAttractions(_identityEntity);
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
