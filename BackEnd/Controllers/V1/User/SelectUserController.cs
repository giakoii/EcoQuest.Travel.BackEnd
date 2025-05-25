// using Client.SystemClient;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using NLog;
// using ProfileService.Services;
// using ProfileService.Utils.Const;
//
// namespace ProfileService.Controllers.V1.Profile;
//
// /// <summary>
// /// Ps010SelectProfileController - Select User Profile
// /// </summary>
// [ApiController]
// [Route("api/v1/[controller]")]
// public class Ps010SelectProfileController : AbstractApiGetController<SelectUserRequest, SelectUserResponse, SelectUserEntity>
// {
//     private readonly IProfileService _profileService;
//     private readonly Logger _logger = LogManager.GetCurrentClassLogger();
//
//     /// <summary>
//     /// Constructor
//     /// </summary>
//     /// <param name="profileService"></param>
//     /// <param name="identityApiClient"></param>
//     public Ps010SelectProfileController(IProfileService profileService, IIdentityApiClient identityApiClient)
//     {
//         _profileService = profileService;
//         _identityApiClient = identityApiClient;
//     }
//     
//     /// <summary>
//     /// Incoming Get
//     /// </summary>
//     /// <param name="request"></param>
//     /// <returns></returns>
//     [HttpGet]
//     [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
//     public override SelectUserResponse Get(SelectUserRequest request)
//     {
//         return Get(request, _logger, new SelectUserResponse());
//     }
//
//     /// <summary>
//     /// Main processing
//     /// </summary>
//     /// <param name="request"></param>
//     /// <returns></returns>
//     protected override SelectUserResponse Exec(SelectUserRequest request)
//     {
//         return _profileService.SelectProfile(_identityEntity);
//     }
//
//     /// <summary>
//     /// Error Check
//     /// </summary>
//     /// <param name="request"></param>
//     /// <param name="detailErrorList"></param>
//     /// <returns></returns>
//     protected internal override SelectUserResponse ErrorCheck(SelectUserRequest request, List<DetailError> detailErrorList)
//     {
//         var response = new SelectUserResponse() { Success = false };
//         if (detailErrorList.Count > 0)
//         {
//             // Error
//             response.SetMessage(MessageId.E10000);
//             response.DetailErrorList = detailErrorList;
//             return response;
//         }
//         // True
//         response.Success = true;
//         return response;
//     }
// }