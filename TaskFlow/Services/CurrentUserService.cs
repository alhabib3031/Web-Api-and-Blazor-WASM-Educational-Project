using System.Security.Claims;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user is null)
                return null;

            var nameIdentifier =
                user.FindFirst("sub")?.Value
                ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("id")?.Value;

            return int.TryParse(nameIdentifier, out var id) ? id : null;
        }
    }
}
