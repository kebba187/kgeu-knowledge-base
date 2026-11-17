using System.Security.Claims;

namespace KbKgeu.Api.Auth;

/// <summary>Кто выполняет запрос — читаем из claims, а не из тела запроса.</summary>
public static class CurrentUser
{
    public static int? GetId(ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var id) ? id : null;
    }

    public static string? GetRole(ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.Role);
}
