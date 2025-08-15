using BackEnd.DTOs.Ecq310;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using OpenIddict.Validation.AspNetCore;

namespace BackEnd.Controllers.V1.Ecq310;

[ApiController]
[Route("api/v1/admin/[controller]")]
public class Ecq310SelectPaymentsController : AbstractApiAsyncController<Ecq310SelectPaymentsRequest, Ecq310SelectPaymentsResponse, PagedResult<Ecq310SelectPaymentsEntity>>
{
    private readonly IPaymentService _paymentService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="paymentService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq310SelectPaymentsController(IPaymentService paymentService, IIdentityApiClient identityApiClient)
    {
        _identityApiClient = identityApiClient;
        _paymentService = paymentService;
    }

    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq310SelectPaymentsResponse> ProcessRequest([FromQuery] Ecq310SelectPaymentsRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq310SelectPaymentsResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq310SelectPaymentsResponse> Exec(Ecq310SelectPaymentsRequest request)
    {
        return await _paymentService.SelectPaymentsAdminDashBoard(request);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq310SelectPaymentsResponse ErrorCheck(Ecq310SelectPaymentsRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq310SelectPaymentsResponse() { Success = false };
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