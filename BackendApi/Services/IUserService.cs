using System.Security.Claims;

namespace BackendApi.Services;

public interface IUserService
{
    public ClaimsPrincipal User { get; set; }
    Claim? GetSubClaim();
}
