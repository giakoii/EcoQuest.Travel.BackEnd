// using Client.SystemClient;
// using BackEnd.Controllers;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using NLog;
// using ProfileService.Services;
// using ProfileService.Utils.Const;
//
// namespace ProfileService.Controllers.V1.Profile;
//
// public class Ps010UpdateProfileController : AbstractApiAsyncController<UpdateUserRequest, UpdateUserResponse, string>
// {
//     private readonly IProfileService _profileService;
//     private readonly Logger _logger = LogManager.GetCurrentClassLogger();
//
//     /// <summary>
//     /// Constructor
//     /// </summary>
//     /// <param name="profileService"></param>
//     /// <param name="identityApiClient"></param>
//     public Ps010UpdateProfileController(IProfileService profileService, IIdentityApiClient identityApiClient)
//     {
//         _profileService = profileService;
//         _identityApiClient = identityApiClient;
//     }
//
//     /// <summary>
//     /// Incoming Put
//     /// </summary>
//     /// <param name="request"></param>
//     /// <returns></returns>
//     [HttpPut]
//     [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
//     public override async Task<UpdateUserResponse> ProcessRequest(UpdateUserRequest request)
//     {
//         return await ProcessRequest(request, _logger, new UpdateUserResponse());
//     }
//
//     protected override async Task<UpdateUserResponse> Exec(UpdateUserRequest request)
//     {
//         return await _profileService.UpdateProfile(request, _identityEntity);
//     }
//
//     /// <summary>
//     /// Error Check
//     /// </summary>
//     /// <param name="request"></param>
//     /// <param name="detailErrorList"></param>
//     /// <returns></returns>
//     protected internal override UpdateUserResponse ErrorCheck(UpdateUserRequest request, List<DetailError> detailErrorList)
//     {
//         var response = new UpdateUserResponse() { Success = false };
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