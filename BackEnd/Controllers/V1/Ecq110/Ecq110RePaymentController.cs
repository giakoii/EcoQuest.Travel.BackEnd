using BackEnd.DTOs.Ecq110;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq110;

[ApiController]
[Route("api/v1/[controller]")]
public class Ecq110RePaymentController : AbstractApiAsyncController<Ecq110RePaymentRequest, Ecq110RePaymentResponse, Ecq110RePaymentEntity>
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IPaymentService _paymentService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="paymentService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq110RePaymentController(IPaymentService paymentService, IIdentityApiClient identityApiClient)
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
    public override Task<Ecq110RePaymentResponse> ProcessRequest(Ecq110RePaymentRequest request)
    {
        return ProcessRequest(request, _logger, new Ecq110RePaymentResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override Task<Ecq110RePaymentResponse> Exec(Ecq110RePaymentRequest request)
    {
        return _paymentService.RePaymentTrip(request, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq110RePaymentResponse ErrorCheck(Ecq110RePaymentRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq110RePaymentResponse() { Success = false };
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