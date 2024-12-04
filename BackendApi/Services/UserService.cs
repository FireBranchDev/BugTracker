using System.Security.Claims;

namespace BackendApi.Services;

public class UserService : IUserService
{
    public ClaimsPrincipal User { get; set; } = null!;

    public Claim? GetSubClaim()
    {
        return User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
    }
}
