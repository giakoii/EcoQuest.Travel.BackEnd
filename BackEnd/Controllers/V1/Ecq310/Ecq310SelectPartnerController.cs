using BackEnd.DTOs.Ecq310;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq310;

/// <summary>
/// Ecq310SelectPartnerController - Select partner
/// </summary>
[Authorize(Policy = "Admin")]
[Route("api/v1/admin/[controller]")]
[ApiController]
public class Ecq310SelectPartnerController : AbstractApiGetAsyncController<Ecq310SelectPartnerRequest, Ecq310SelectPartnerResponse, Ecq310SelectPartnerEntity>
{
    private readonly IPartnerService _partnerService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="partnerService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq310SelectPartnerController(IPartnerService partnerService, IIdentityApiClient identityApiClient)
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
    public override async Task<Ecq310SelectPartnerResponse> Get(Ecq310SelectPartnerRequest request)
    {
        return await Get(request, _logger, new Ecq310SelectPartnerResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq310SelectPartnerResponse> Exec(Ecq310SelectPartnerRequest request)
    {
        return await _partnerService.SelectPartner(request.PartnerId);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq310SelectPartnerResponse ErrorCheck(Ecq310SelectPartnerRequest request,
        List<DetailError> detailErrorList)
    {
        var response = new Ecq310SelectPartnerResponse() { Success = false };
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