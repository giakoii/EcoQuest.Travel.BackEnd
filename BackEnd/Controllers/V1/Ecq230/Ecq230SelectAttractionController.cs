using BackEnd.DTOs.Ecq230;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq230;

/// <summary>
/// Ecq230SelectAttractionController - Select attraction
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq230SelectAttractionController : AbstractApiAsyncController<Ecq230SelectAttractionRequest, Ecq230SelectAttractionResponse, Ecq230AttractionDetailEntity>
{
    private readonly IAttractionService _attractionService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attractionService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq230SelectAttractionController(IAttractionService attractionService, IIdentityApiClient identityApiClient)
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
    public override async Task<Ecq230SelectAttractionResponse> ProcessRequest([FromQuery] Ecq230SelectAttractionRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq230SelectAttractionResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq230SelectAttractionResponse> Exec(Ecq230SelectAttractionRequest request)
    {
        return await _attractionService.Ecq230SelectAttraction(request.AttractionId, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq230SelectAttractionResponse ErrorCheck(Ecq230SelectAttractionRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq230SelectAttractionResponse() { Success = false };
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
