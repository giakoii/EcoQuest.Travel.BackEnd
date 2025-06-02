using BackEnd.DTOs.Ecq310;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq310;

/// <summary>
/// Ecq310UpdatePartnerController - Update partner
/// </summary>
[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Policy = "Admin")]
public class Ecq310UpdatePartnerController : AbstractApiAsyncController<Ecq310UpdatePartnerRequest, Ecq310UpdatePartnerResponse, string>
{
    private readonly IPartnerService _partnerService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="partnerService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq310UpdatePartnerController(IPartnerService partnerService, IIdentityApiClient identityApiClient)
    {
        _partnerService = partnerService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Put
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    public override async Task<Ecq310UpdatePartnerResponse> ProcessRequest(Ecq310UpdatePartnerRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq310UpdatePartnerResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq310UpdatePartnerResponse> Exec(Ecq310UpdatePartnerRequest request)
    {
        return await _partnerService.UpdatePartner(request, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq310UpdatePartnerResponse ErrorCheck(Ecq310UpdatePartnerRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq310UpdatePartnerResponse() { Success = false };
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