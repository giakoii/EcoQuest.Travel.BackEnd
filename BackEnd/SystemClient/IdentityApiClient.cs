using System.Security.Claims;
using OpenIddict.Abstractions;

namespace BackEnd.SystemClient;

public class IdentityApiClient : IIdentityApiClient
{
    public IdentityEntity GetIdentity(ClaimsPrincipal user)
    {
        var identity = user.Identity as ClaimsIdentity;
        
        // Get id
        var id = identity?.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        
        // Get username
        var userNm = identity.FindFirst("UserId")?.Value ?? string.Empty;
        
        // Get email
        var email = identity.FindFirst(OpenIddictConstants.Claims.Email)?.Value ?? string.Empty;
        
        // Get phone number
        var phoneNumber = identity.FindFirst(OpenIddictConstants.Claims.PhoneNumber)?.Value ?? string.Empty;
        
        // Create IdentityEntity
        var identityEntity = new IdentityEntity
        {
            UserId = id,
            Email = email,
        };
        return identityEntity;
    }
}