using BackEnd.DTOs.Ecq220;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq220;

/// <summary>
/// Ecq220InsertRestaurantController - Insert restaurant
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq220InsertRestaurantController : AbstractApiAsyncController<Ecq220InsertRestaurantRequest, Ecq220InsertRestaurantResponse, string>
{
    private readonly IRestaurantService _restaurantService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="restaurantService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq220InsertRestaurantController(IRestaurantService restaurantService, IIdentityApiClient identityApiClient)
    {
        _restaurantService = restaurantService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Post
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = ConstRole.Partner, AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq220InsertRestaurantResponse> ProcessRequest([FromForm] Ecq220InsertRestaurantRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq220InsertRestaurantResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq220InsertRestaurantResponse> Exec(Ecq220InsertRestaurantRequest request)
    {
        return await _restaurantService.InsertRestaurant(request, _identityEntity);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq220InsertRestaurantResponse ErrorCheck(Ecq220InsertRestaurantRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq220InsertRestaurantResponse() { Success = false };
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
