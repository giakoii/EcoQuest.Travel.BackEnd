using System.Security.Claims;
using BackEnd.DTOs.Account;
using BackEnd.Helpers;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace BackEnd.Controllers.V1.Authentication;

/// <summary>
/// Authentication controller - Exchange token
/// </summary>
public class AuthenticationController : ControllerBase
{
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly IUserService _userService;
    private readonly ITempCodeService _tempCodeService;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="scopeManager"></param>
    /// <param name="userService"></param>
    /// <param name="tempCodeService"></param>
    public AuthenticationController(IOpenIddictScopeManager scopeManager, IUserService userService,
        ITempCodeService tempCodeService)
    {
        _scopeManager = scopeManager;
        _userService = userService;
        _tempCodeService = tempCodeService;
    }

    /// <summary>
    /// Exchange token
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("~/connect/token")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange(AuthRequest request)
    {
        var openIdRequest = HttpContext.GetOpenIddictServerRequest();

        // Password
        if (openIdRequest!.IsPasswordGrantType())
        {
            return await TokensForPasswordGrantType(request);
        }

        // Custom Google flow
        if (openIdRequest!.GrantType == "google")
        {
            return await TokensForGoogleGrantType(request);
        }

        // Refresh token
        if (openIdRequest!.IsRefreshTokenGrantType())
        {
            var claimsPrincipal =
                (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme))
                .Principal;
            return SignIn(claimsPrincipal!, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // Unsupported grant type
        return BadRequest(new OpenIddictResponse
        {
            Error = Errors.UnsupportedGrantType
        });
    }

    [HttpGet("google/login")]
    public IActionResult LoginWithGoogle()
    {
        var redirectUrl = Url.Action("GoogleCallback", "Authentication");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, "Google");
    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        // Authenticate Google response
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (!result.Succeeded)
        {
            return BadRequest("Google authentication failed");
        }

        // Get user information from the claims
        var email = result.Principal!.FindFirstValue(ClaimTypes.Email);
        var firstName = result.Principal!.FindFirstValue(ClaimTypes.GivenName);
        var lastName = result.Principal!.FindFirstValue(ClaimTypes.Surname);
        var avatar = result.Principal!.FindFirstValue("picture");

        // Signin/ Signup user
        var userLogin = await _userService.GoogleLogin(email!, firstName!, lastName!, avatar!);
        if (!userLogin.Success)
        {
            return BadRequest(userLogin);
        }

        // Create temporary code for the user
        var tempCode = Guid.NewGuid().ToString();

        // Insert temporary code into the database
        await _tempCodeService.SaveUserInfo(tempCode, new TempUserInfo
        {
            Email = userLogin.Response.Email,
            RoleName = userLogin.Response.RoleName,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        });

        // Redirect to the frontend with the temporary code
        return Redirect($"{ConstUrl.UrlFrontEnd}?code={tempCode}&state=google_login");
    }

    /// <summary>
    /// Generate tokens for the user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IActionResult> TokensForPasswordGrantType(AuthRequest request)
    {
        // Else user use password login
        var userPasswordLogin = await _userService.AuthLogin(request);
        if (userPasswordLogin.Success == false)
        {
            return Unauthorized(new OpenIddictResponse
            {
                Error = Errors.InvalidGrant,
                ErrorDescription = userPasswordLogin.Message,
            });
        }

        // Create claims
        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            Claims.Name,
            Claims.Role
        );

        // Set claims
        identity.SetClaim(Claims.Subject, userPasswordLogin.Response.AccountId.ToString(),
            Destinations.AccessToken);
        identity.SetClaim(Claims.Name, userPasswordLogin.Response.Name,
            Destinations.AccessToken);
        identity.SetClaim("UserId", userPasswordLogin.Response.AccountId.ToString(),
            Destinations.AccessToken);
        identity.SetClaim(Claims.Email, userPasswordLogin.Response.Email,
            Destinations.AccessToken);       
        identity.SetClaim(Claims.PhoneNumber, userPasswordLogin.Response.Email,
            Destinations.AccessToken);
        identity.SetClaim(Claims.Role, userPasswordLogin.Response.RoleName,
            Destinations.AccessToken);
        identity.SetClaim(Claims.Audience, "service_client",
            Destinations.AccessToken);

        identity.SetDestinations(claim =>
        {
            return claim.Type switch
            {
                Claims.Subject => new[] { Destinations.AccessToken },
                Claims.Name => new[] { Destinations.AccessToken },
                "UserId" => new[] { Destinations.AccessToken },
                Claims.Email => new[] { Destinations.AccessToken },
                Claims.Role => new[] { Destinations.AccessToken },
                Claims.Audience => new[] { Destinations.AccessToken },
                _ => new[] { Destinations.AccessToken }
            };
        });

        // Set scopes
        var claimsPrincipal = new ClaimsPrincipal(identity);
        claimsPrincipal.SetScopes(new string[]
        {
            Scopes.Roles,
            Scopes.OfflineAccess,
            Scopes.Profile,
        });

        claimsPrincipal.SetResources(await _scopeManager.ListResourcesAsync(claimsPrincipal.GetScopes()).ToListAsync());

        // Set refresh token and access token
        claimsPrincipal.SetAccessTokenLifetime(TimeSpan.FromMinutes(60));
        claimsPrincipal.SetRefreshTokenLifetime(TimeSpan.FromMinutes(120));

        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Generate tokens for the user using Google grant type
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task<IActionResult> TokensForGoogleGrantType(AuthRequest request)
    {
        // Lấy code từ request
        var code = request.Code;

        if (string.IsNullOrEmpty(code))
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.InvalidRequest,
                ErrorDescription = "Missing authorization code"
            });
        }

        // Lấy thông tin user từ temp code
        var userInfo = await _tempCodeService.GetUserInfo(code);
        if (userInfo == null || userInfo.ExpiresAt < DateTime.UtcNow)
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.InvalidGrant,
                ErrorDescription = "Invalid or expired authorization code"
            });
        }

        // Delete the temporary code
        await _tempCodeService.RemoveCode(code);

        // Create claims for the user
        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            Claims.Name,
            Claims.Role
        );

        // Set claims
        identity.SetClaim(Claims.Subject, userInfo.Email,
            Destinations.AccessToken);
        identity.SetClaim(Claims.Name, userInfo.Email,
            Destinations.AccessToken);
        identity.SetClaim(Claims.Email, userInfo.Email,
            Destinations.AccessToken);
        identity.SetClaim(Claims.Role, userInfo.RoleName,
            Destinations.AccessToken);
        identity.SetClaim(Claims.Audience, "service_client",
            Destinations.AccessToken);

        identity.SetDestinations(claim =>
        {
            return claim.Type switch
            {
                Claims.Subject => new[] { Destinations.AccessToken },
                Claims.Name => new[] { Destinations.AccessToken },
                Claims.Email => new[] { Destinations.AccessToken },
                Claims.Role => new[] { Destinations.AccessToken },
                Claims.Audience => new[] { Destinations.AccessToken },
                _ => new[] { Destinations.AccessToken }
            };
        });

        var claimsPrincipal = new ClaimsPrincipal(identity);
        claimsPrincipal.SetScopes(new string[]
        {
            Scopes.Roles,
            Scopes.OfflineAccess,
            Scopes.Profile,
        });

        claimsPrincipal.SetResources(await _scopeManager.ListResourcesAsync(claimsPrincipal.GetScopes()).ToListAsync());

        claimsPrincipal.SetAccessTokenLifetime(TimeSpan.FromMinutes(60));
        claimsPrincipal.SetRefreshTokenLifetime(TimeSpan.FromMinutes(120));

        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}