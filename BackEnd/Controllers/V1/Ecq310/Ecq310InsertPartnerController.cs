using BackEnd.DTOs.Ecq310;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq310;

/// <summary>
/// Ecq310InsertPartnerController - Insert new partner
/// </summary>
[Authorize(Policy = "Admin")]
[Route("api/v1/admin/[controller]")]
[ApiController]
public class Ecq310InsertPartnerController : AbstractApiAsyncController<Ecq310InsertPartnerRequest, Ecq310InsertPartnerResponse, string>
{
    private readonly IPartnerService _partnerService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="partnerService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq310InsertPartnerController(IPartnerService partnerService, IIdentityApiClient identityApiClient)
    {
        _partnerService = partnerService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Post
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public override async Task<Ecq310InsertPartnerResponse> ProcessRequest(Ecq310InsertPartnerRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq310InsertPartnerResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override Task<Ecq310InsertPartnerResponse> Exec(Ecq310InsertPartnerRequest request)
    {
        return _partnerService.InsertPartner(request, _identityEntity);
    }
    
    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq310InsertPartnerResponse ErrorCheck(Ecq310InsertPartnerRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq310InsertPartnerResponse() { Success = false };
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