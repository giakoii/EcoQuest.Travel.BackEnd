using BackEnd.DTOs.Ecq110;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq110;

/// <summary>
/// Ecq110InsertPaymentController - Insert a new payment for booking
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq110InsertPaymentController : AbstractApiAsyncController<Ecq110InsertPaymentRequest, Ecq110InsertPaymentResponse, Ecq110InsertPaymentEntity>
{
    private readonly IPaymentService _paymentService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="paymentService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq110InsertPaymentController(IPaymentService paymentService, IIdentityApiClient identityApiClient)
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
    public override async Task<Ecq110InsertPaymentResponse> ProcessRequest(Ecq110InsertPaymentRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq110InsertPaymentResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq110InsertPaymentResponse> Exec(Ecq110InsertPaymentRequest request)
    {
        return await _paymentService.InsertPayment(request, _identityEntity);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq110InsertPaymentResponse ErrorCheck(Ecq110InsertPaymentRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq110InsertPaymentResponse { Success = false };
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