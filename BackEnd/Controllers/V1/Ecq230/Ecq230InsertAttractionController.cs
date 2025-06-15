using BackEnd.DTOs.Ecq230;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq230;

/// <summary>
/// Ecq230InsertAttractionController - Insert attraction
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq230InsertAttractionController : AbstractApiAsyncController<Ecq230InsertAttractionRequest, Ecq230InsertAttractionResponse, string>
{
    private readonly IAttractionService _attractionService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attractionService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq230InsertAttractionController(IAttractionService attractionService, IIdentityApiClient identityApiClient)
    {
        _attractionService = attractionService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Post
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq230InsertAttractionResponse> ProcessRequest([FromForm] Ecq230InsertAttractionRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq230InsertAttractionResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq230InsertAttractionResponse> Exec(Ecq230InsertAttractionRequest request)
    {
        return await _attractionService.InsertAttraction(request, _identityEntity);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq230InsertAttractionResponse ErrorCheck(Ecq230InsertAttractionRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq230InsertAttractionResponse() { Success = false };
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
