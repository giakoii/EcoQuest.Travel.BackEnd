using BackEnd.DTOs.Ecq310;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq310;

[ApiController]
[Route("api/v1/admin/[controller]")]
public class Ecq310SelectPaymentController : AbstractApiAsyncController<Ecq310SelectPaymentRequest, Ecq310SelectPaymentResponse,Ecq310SelectPaymentEntity>
{
    private readonly IPaymentService _paymentService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="paymentService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq310SelectPaymentController(IPaymentService paymentService, IIdentityApiClient identityApiClient)
    {
        _paymentService = paymentService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    
    public override async Task<Ecq310SelectPaymentResponse> ProcessRequest([FromQuery] Ecq310SelectPaymentRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq310SelectPaymentResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq310SelectPaymentResponse> Exec(Ecq310SelectPaymentRequest request)
    {
        return await _paymentService.SelectPaymentAdminDashBoard(request);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq310SelectPaymentResponse ErrorCheck(Ecq310SelectPaymentRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq310SelectPaymentResponse() { Success = false };
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