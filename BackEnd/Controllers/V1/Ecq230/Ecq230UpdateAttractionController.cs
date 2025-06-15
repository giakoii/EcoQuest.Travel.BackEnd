using BackEnd.DTOs.Ecq230;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq230;

/// <summary>
/// Ecq230UpdateAttractionController - Update attraction
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq230UpdateAttractionController : AbstractApiAsyncController<Ecq230UpdateAttractionRequest, Ecq230UpdateAttractionResponse, string>
{
    private readonly IAttractionService _attractionService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attractionService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq230UpdateAttractionController(IAttractionService attractionService, IIdentityApiClient identityApiClient)
    {
        _attractionService = attractionService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Put
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq230UpdateAttractionResponse> ProcessRequest([FromForm] Ecq230UpdateAttractionRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq230UpdateAttractionResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq230UpdateAttractionResponse> Exec(Ecq230UpdateAttractionRequest request)
    {
        return await _attractionService.Ecq230UpdateAttraction(request, _identityEntity);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq230UpdateAttractionResponse ErrorCheck(Ecq230UpdateAttractionRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq230UpdateAttractionResponse() { Success = false };
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
