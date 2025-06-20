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

namespace BackEnd.Controllers.V1.Ecq010;

/// <summary>
/// Authentication controller - Exchange token
/// </summary>
public class Ecq010AuthenticationController : ControllerBase
{
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly IUserService _userService;
    private readonly ITempCodeService _tempCodeService;

    /// <summary>
    /// Constructor
    /// </summary>
    public Ecq010AuthenticationController(IOpenIddictScopeManager scopeManager, IUserService userService, ITempCodeService tempCodeService)
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
        var redirectUrl = Url.Action("GoogleCallback", "Ecq010Authentication");
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
        if (!userPasswordLogin.Success)
        {
            return Unauthorized(new OpenIddictResponse
            {
                Error = Errors.InvalidRequest,
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
        identity.SetClaim(Claims.Address, userPasswordLogin.Response.Address,
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
    //
    // /// <summary>
    // /// Logout endpoint - Handles both GET and POST requests
    // /// </summary>
    // /// <returns></returns>
    // [HttpPost("~/connect/logout")]
    // [HttpGet("~/connect/logout")]
    // public async Task<IActionResult> Logout()
    // {
    //     try
    //     {
    //         // Get the OpenIddict server request
    //         var authenticateResult = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    //
    //         // If user is authenticated, perform logout operations
    //         if (authenticateResult.Succeeded)
    //         {
    //             var claimsPrincipal = authenticateResult.Principal;
    //             var identity = claimsPrincipal.Identity as ClaimsIdentity;
    //
    //             if (identity != null)
    //             {
    //
    //                 // Get user information for logging
    //                 var userEmail = identity.FindFirst(OpenIddictConstants.Claims.Email)?.Value;
    //                 var userId = identity.FindFirst("UserId")?.Value;
    //
    //
    //                 _logger.LogInformation("User logout initiated for Email: {Email}, UserId: {UserId}",
    //                     userEmail, userId);
    //
    //                 // Revoke all tokens for the current user
    //                 await RevokeAllUserTokensAsync(claimsPrincipal);
    //
    //                 // Sign out from cookie authentication if using cookies
    //                 await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    //
    //                 // Clear any distributed cache entries related to the user
    //                 if (!string.IsNullOrEmpty(userId))
    //                 {
    //                     await ClearUserCacheAsync(userId);
    //                 }
    //
    //                 _logger.LogInformation("User logout completed successfully for Email: {Email}", userEmail);
    //             }
    //         }
    //
    //         // Handle post logout redirect
    //         // if (!string.IsNullOrEmpty(request?.PostLogoutRedirectUri))
    //         // {
    //         //     // Validate the redirect URI against registered client URIs
    //         //     if (await IsValidPostLogoutRedirectUriAsync(request.PostLogoutRedirectUri))
    //         //     {
    //         //         return Redirect(request.PostLogoutRedirectUri);
    //         //     }
    //         //     else
    //         //     {
    //         //         _logger.LogWarning("Invalid post logout redirect URI: {Uri}", request.PostLogoutRedirectUri);
    //         //         return BadRequest(new OpenIddictResponse
    //         //         {
    //         //             Error = Errors.InvalidRequest,
    //         //             ErrorDescription = "Invalid post_logout_redirect_uri"
    //         //         });
    //         //     }
    //         // }
    //
    //         // Return success response if no redirect URI
    //         return Ok(new { message = "Logout successful" });
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error occurred during logout process");
    //         return StatusCode(500, new OpenIddictResponse
    //         {
    //             Error = Errors.ServerError,
    //             ErrorDescription = "An error occurred during logout"
    //         });
    //     }
    // }
    //
    // /// <summary>
    // /// End session endpoint - Alternative logout endpoint following OpenID Connect standard
    // /// </summary>
    // /// <returns></returns>
    // [HttpGet("~/connect/endsession")]
    // [HttpPost("~/connect/endsession")]
    // public async Task<IActionResult> EndSession()
    // {
    //     try
    //     {
    //         var request = HttpContext.GetOpenIddictServerRequest();
    //
    //         // Validate ID token hint if provided
    //         if (!string.IsNullOrEmpty(request?.IdTokenHint))
    //         {
    //             var isValidIdToken = await ValidateIdTokenHintAsync(request.IdTokenHint);
    //             if (!isValidIdToken)
    //             {
    //                 return BadRequest(new OpenIddictResponse
    //                 {
    //                     Error = Errors.InvalidRequest,
    //                     ErrorDescription = "Invalid id_token_hint"
    //                 });
    //             }
    //         }
    //
    //         // Perform logout
    //         var claimsPrincipal = HttpContext.User;
    //         if (claimsPrincipal.Identity?.IsAuthenticated == true)
    //         {
    //             var userEmail = claimsPrincipal.FindFirst(Claims.Email)?.Value;
    //             var userId = claimsPrincipal.FindFirst("UserId")?.Value;
    //
    //             _logger.LogInformation("End session initiated for Email: {Email}, UserId: {UserId}",
    //                 userEmail, userId);
    //
    //             await RevokeAllUserTokensAsync(claimsPrincipal);
    //             await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    //
    //             if (!string.IsNullOrEmpty(userId))
    //             {
    //                 await ClearUserCacheAsync(userId);
    //             }
    //         }
    //
    //         // Handle post logout redirect
    //         if (!string.IsNullOrEmpty(request?.PostLogoutRedirectUri))
    //         {
    //             if (await IsValidPostLogoutRedirectUriAsync(request.PostLogoutRedirectUri))
    //             {
    //                 return Redirect(request.PostLogoutRedirectUri);
    //             }
    //             else
    //             {
    //                 return BadRequest(new OpenIddictResponse
    //                 {
    //                     Error = Errors.InvalidRequest,
    //                     ErrorDescription = "Invalid post_logout_redirect_uri"
    //                 });
    //             }
    //         }
    //
    //         return Ok(new { message = "End session successful" });
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error occurred during end session process");
    //         return StatusCode(500, new OpenIddictResponse
    //         {
    //             Error = Errors.ServerError,
    //             ErrorDescription = "An error occurred during end session"
    //         });
    //     }
    // }
    //
    // /// <summary>
    // /// Token revocation endpoint
    // /// </summary>
    // /// <returns></returns>
    // [HttpPost("~/connect/revoke")]
    // [Consumes("application/x-www-form-urlencoded")]
    // public async Task<IActionResult> Revoke()
    // {
    //     try
    //     {
    //         var request = HttpContext.GetOpenIddictServerRequest();
    //
    //         if (string.IsNullOrEmpty(request?.Token))
    //         {
    //             return BadRequest(new OpenIddictResponse
    //             {
    //                 Error = Errors.InvalidRequest,
    //                 ErrorDescription = "Missing token parameter"
    //             });
    //         }
    //
    //         // Find and revoke the token
    //         var token = await _tokenManager.FindByReferenceIdAsync(request.Token);
    //         if (token != null)
    //         {
    //             await _tokenManager.TryRevokeAsync(token);
    //             _logger.LogInformation("Token revoked successfully");
    //         }
    //
    //         // Always return success even if token not found (per OAuth 2.0 spec)
    //         return Ok();
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error occurred during token revocation");
    //         return StatusCode(500, new OpenIddictResponse
    //         {
    //             Error = Errors.ServerError,
    //             ErrorDescription = "An error occurred during token revocation"
    //         });
    //     }
    // }
    //
    // /// <summary>
    // /// Revoke all tokens for a specific user
    // /// </summary>
    // /// <param name="claimsPrincipal"></param>
    // /// <returns></returns>
    // private async Task RevokeAllUserTokensAsync(ClaimsPrincipal claimsPrincipal)
    // {
    //     try
    //     {
    //         var userId = claimsPrincipal.FindFirst("UserId")?.Value ??
    //                      claimsPrincipal.FindFirst(Claims.Subject)?.Value;
    //
    //         if (string.IsNullOrEmpty(userId))
    //         {
    //             _logger.LogWarning("No user ID found in claims for token revocation");
    //             return;
    //         }
    //
    //         // Find all tokens for the user
    //         var tokens = _tokenManager.FindBySubjectAsync(userId);
    //
    //         await foreach (var token in tokens)
    //         {
    //             // Check if token is already revoked to avoid unnecessary operations
    //             var status = await _tokenManager.GetStatusAsync(token);
    //             if (status == Statuses.Valid)
    //             {
    //                 // Use TryRevokeAsync which handles the revocation safely
    //                 await _tokenManager.TryRevokeAsync(token);
    //             }
    //         }
    //
    //         _logger.LogInformation("All tokens revoked for user: {UserId}", userId);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error revoking user tokens");
    //     }
    // }
    //
    // /// <summary>
    // /// Clear user-related cache entries
    // /// </summary>
    // /// <param name="userId"></param>
    // /// <returns></returns>
    // private async Task ClearUserCacheAsync(string userId)
    // {
    //     try
    //     {
    //         // Clear user-specific cache entries
    //         var cacheKeys = new[]
    //         {
    //             $"user:{userId}",
    //             $"user_permissions:{userId}",
    //             $"user_roles:{userId}",
    //             $"user_profile:{userId}"
    //         };
    //
    //         foreach (var key in cacheKeys)
    //         {
    //             await _distributedCache.RemoveAsync(key);
    //         }
    //
    //         _logger.LogInformation("Cache cleared for user: {UserId}", userId);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error clearing user cache for user: {UserId}", userId);
    //     }
    // }
    //
    // /// <summary>
    // /// Validate post logout redirect URI
    // /// </summary>
    // /// <param name="redirectUri"></param>
    // /// <returns></returns>
    // private async Task<bool> IsValidPostLogoutRedirectUriAsync(string redirectUri)
    // {
    //     try
    //     {
    //         // Get all registered applications
    //         var applications = _applicationManager.ListAsync();
    //
    //         await foreach (var application in applications)
    //         {
    //             var postLogoutRedirectUris = await _applicationManager.GetPostLogoutRedirectUrisAsync(application);
    //
    //             if (postLogoutRedirectUris.Contains(redirectUri, StringComparer.OrdinalIgnoreCase))
    //             {
    //                 return true;
    //             }
    //         }
    //
    //         // Also check against frontend URL from constants
    //         if (redirectUri.StartsWith(ConstUrl.UrlFrontEnd, StringComparison.OrdinalIgnoreCase))
    //         {
    //             return true;
    //         }
    //
    //         return false;
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error validating post logout redirect URI: {Uri}", redirectUri);
    //         return false;
    //     }
    // }
    //
    // /// <summary>
    // /// Validate ID token hint
    // /// </summary>
    // /// <param name="idTokenHint"></param>
    // /// <returns></returns>
    // private async Task<bool> ValidateIdTokenHintAsync(string idTokenHint)
    // {
    //     try
    //     {
    //         // Find token by reference ID or value
    //         var token = await _tokenManager.FindByReferenceIdAsync(idTokenHint) ??
    //                     await _tokenManager.FindByIdAsync(idTokenHint);
    //
    //         if (token == null)
    //         {
    //             return false;
    //         }
    //
    //         // Check if token status is valid (not revoked)
    //         var status = await _tokenManager.GetStatusAsync(token);
    //         if (status != Statuses.Valid)
    //         {
    //             return false;
    //         }
    //
    //         // Check if token has expired
    //         var expirationDate = await _tokenManager.GetExpirationDateAsync(token);
    //         if (expirationDate.HasValue && expirationDate.Value < DateTimeOffset.UtcNow)
    //         {
    //             return false;
    //         }
    //
    //         // Additional check: verify token type is ID token
    //         var tokenType = await _tokenManager.GetTypeAsync(token);
    //         return tokenType == TokenTypes.Bearer;
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error validating ID token hint");
    //         return false;
    //     }
    // }
}