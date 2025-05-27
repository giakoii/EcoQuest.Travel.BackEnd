using System.Security.Claims;
using BackEnd.DTOs.Account;
using BackEnd.Helpers;
using BackEnd.Models.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace BackEnd.Controllers.V1.Authentication;

/// <summary>
/// Authentication controller - Exchange token
/// </summary>
public class AuthenticationController : ControllerBase
{
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly AppDbContext _context;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="scopeManager"></param>
    /// <param name="context"></param>
    public AuthenticationController(IOpenIddictScopeManager scopeManager, AppDbContext context)
    {
        _scopeManager = scopeManager;
        _context = context;
    }

    /// <summary>
    ///  Exchange token
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
            Error = OpenIddictConstants.Errors.UnsupportedGrantType
        });
    }

    /// <summary>
    /// Tokens for password grant type
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task<IActionResult> TokensForPasswordGrantType(AuthRequest request)
    {
        // Check user exists
        var userExist = _context.Accounts.FirstOrDefault(x => x.Email == request.Email);
        if (userExist == null)
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.InvalidGrant,
                ErrorDescription = "The username or password is incorrect."
            });
        }

        // Check if User updating
        if (userExist.IsActive == false || userExist.LockoutEnd != null && userExist.Key != null)
        {
            return Unauthorized(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.InvalidGrant,
                ErrorDescription = "Your account has been locked."
            });
        }

        // Check password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, userExist.PasswordHash))
        {
            userExist.AccessFailedCount++;
            if (userExist.AccessFailedCount >= 5)
            {
                userExist.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
            }

            _context.Accounts.Update(userExist);
            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.InvalidGrant,
                ErrorDescription = "The username or password is incorrect."
            });
        }

        // Check lockout
        if (userExist.LockoutEnd != null && userExist.LockoutEnd > DateTime.UtcNow)
        {
            return Unauthorized(new OpenIddictResponse
            {
                ErrorDescription = "Your account has been locked."
            });
        }

        // Check user is active
        if (!userExist.IsActive)
        {
            return Unauthorized();
        }

        userExist.AccessFailedCount = 0;
        userExist.LockoutEnd = null;
        await _context.SaveChangesAsync();

        var role = await _context.Roles.FindAsync(userExist.RoleId);

        // Create claims
        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            OpenIddictConstants.Claims.Name,
            OpenIddictConstants.Claims.Role
        );

        // Set claims
        identity.SetClaim(OpenIddictConstants.Claims.Subject, userExist.AccountId.ToString(), OpenIddictConstants.Destinations.AccessToken);
        identity.SetClaim("UserId", request.Email, OpenIddictConstants.Destinations.AccessToken);
        identity.SetClaim(OpenIddictConstants.Claims.Email, userExist.Email, OpenIddictConstants.Destinations.AccessToken);
        identity.SetClaim(OpenIddictConstants.Claims.Role, role?.Name, OpenIddictConstants.Destinations.AccessToken);
        identity.SetClaim(OpenIddictConstants.Claims.Audience, "service_client", OpenIddictConstants.Destinations.AccessToken);

        identity.SetDestinations(claim =>
        {
            return claim.Type switch
            {
                OpenIddictConstants.Claims.Subject => new[] { OpenIddictConstants.Destinations.AccessToken },
                OpenIddictConstants.Claims.Email => new[] { OpenIddictConstants.Destinations.AccessToken },
                OpenIddictConstants.Claims.Role => new[] { OpenIddictConstants.Destinations.AccessToken },
                OpenIddictConstants.Claims.Audience => new[] { OpenIddictConstants.Destinations.AccessToken },
                _ => new[] { OpenIddictConstants.Destinations.AccessToken }
            };
        });

        // Set scopes
        var claimsPrincipal = new ClaimsPrincipal(identity);
        claimsPrincipal.SetScopes(new string[]
        {
            OpenIddictConstants.Scopes.Roles,
            OpenIddictConstants.Scopes.OfflineAccess,
            OpenIddictConstants.Scopes.Profile,
        });

        claimsPrincipal.SetResources(await _scopeManager.ListResourcesAsync(claimsPrincipal.GetScopes()).ToListAsync());

        // Set refresh token and access token
        claimsPrincipal.SetAccessTokenLifetime(TimeSpan.FromMinutes(60));
        claimsPrincipal.SetRefreshTokenLifetime(TimeSpan.FromMinutes(120));

        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}