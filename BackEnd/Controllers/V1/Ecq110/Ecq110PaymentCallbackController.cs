using BackEnd.DTOs.Ecq110;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq110;

/// <summary>
/// Ecq110PaymentCallbackController - Handling payment callback requests
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq110PaymentCallbackController : AbstractApiAsyncController<Ecq110PaymentCallbackRequest, Ecq110PaymentCallbackResponse, Ecq110PaymentCallbackEntity>
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IPaymentService _paymentService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="paymentService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq110PaymentCallbackController(IPaymentService paymentService, IIdentityApiClient identityApiClient)
    {
        _paymentService = paymentService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Post
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override Task<Ecq110PaymentCallbackResponse> ProcessRequest(Ecq110PaymentCallbackRequest request)
    {
        return ProcessRequest(request, _logger, new Ecq110PaymentCallbackResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override Task<Ecq110PaymentCallbackResponse> Exec(Ecq110PaymentCallbackRequest request)
    {
        return _paymentService.PaymentCallback(request, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq110PaymentCallbackResponse ErrorCheck(Ecq110PaymentCallbackRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq110PaymentCallbackResponse() { Success = false };
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