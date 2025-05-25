using System.Security.Claims;

namespace BackEnd.SystemClient;

public interface IIdentityApiClient
{
    public IdentityEntity? GetIdentity(ClaimsPrincipal user);
}