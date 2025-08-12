using BackEnd.DTOs.Ecq110;
using BackEnd.DTOs.User;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using OpenIddict.Validation.AspNetCore;

namespace BackEnd.Controllers.V1.Ecq300;

[ApiController]
[Route("api/v1/[controller]")]
public class Ecq300PaymentPremierAccountController : AbstractApiAsyncController<Ecq300PaymentPremierAccountRequest, Ecq300PaymentPremierAccountResponse, Ecq110InsertPaymentEntity>
{
    private readonly IPaymentService _paymentService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Incoming Patch
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq300PaymentPremierAccountResponse> ProcessRequest(Ecq300PaymentPremierAccountRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq300PaymentPremierAccountResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq300PaymentPremierAccountResponse> Exec(Ecq300PaymentPremierAccountRequest request)
    {
        return await _paymentService.PaymentPremierAccount(request, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq300PaymentPremierAccountResponse ErrorCheck(Ecq300PaymentPremierAccountRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq300PaymentPremierAccountResponse() { Success = false };
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