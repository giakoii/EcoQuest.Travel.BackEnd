using BackEnd.DTOs.Ecq310;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq310;

/// <summary>
/// Ecq310SelectPartnersController - Select partner list
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Policy = "Admin")]
public class Ecq310SelectPartnersController : AbstractApiAsyncController<Ecq310SelectPartnersRequest, Ecq310SelectPartnersResponse, List<Ecq310SelectPartnersEntity>>
{
    private readonly IPartnerService _partnerService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="partnerService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq310SelectPartnersController(IPartnerService partnerService, IIdentityApiClient identityApiClient)
    {
        _partnerService = partnerService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public override async Task<Ecq310SelectPartnersResponse> ProcessRequest([FromQuery] Ecq310SelectPartnersRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq310SelectPartnersResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq310SelectPartnersResponse> Exec(Ecq310SelectPartnersRequest request)
    {
        return await _partnerService.SelectPartners(request);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq310SelectPartnersResponse ErrorCheck(Ecq310SelectPartnersRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq310SelectPartnersResponse() { Success = false };
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