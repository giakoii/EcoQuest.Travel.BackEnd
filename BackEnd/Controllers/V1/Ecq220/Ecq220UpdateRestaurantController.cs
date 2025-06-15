using BackEnd.DTOs.Ecq220;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq220;

/// <summary>
/// Ecq220UpdateRestaurantController - Update restaurant
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq220UpdateRestaurantController : AbstractApiAsyncController<Ecq220UpdateRestaurantRequest, Ecq220UpdateRestaurantResponse, string>
{
    private readonly IRestaurantService _restaurantService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="restaurantService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq220UpdateRestaurantController(IRestaurantService restaurantService, IIdentityApiClient identityApiClient)
    {
        _restaurantService = restaurantService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Put
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(Roles = ConstRole.Partner, AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq220UpdateRestaurantResponse> ProcessRequest([FromForm] Ecq220UpdateRestaurantRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq220UpdateRestaurantResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq220UpdateRestaurantResponse> Exec(Ecq220UpdateRestaurantRequest request)
    {
        return await _restaurantService.UpdateRestaurant(request, _identityEntity);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq220UpdateRestaurantResponse ErrorCheck(Ecq220UpdateRestaurantRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq220UpdateRestaurantResponse() { Success = false };
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
