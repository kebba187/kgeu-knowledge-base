using System.Security.Claims;
using KbKgeu.Api.Dtos;
using KbKgeu.Api.Models;
using KbKgeu.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KbKgeu.Api.Auth;

/// <summary>Вход, выход и сведения о текущем пользователе.</summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IUserStore _users;
    private readonly IPasswordHasher<User> _hasher;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserStore users,
        IPasswordHasher<User> hasher,
        ILogger<AuthController> logger)
    {
        _users = users;
        _hasher = hasher;
        _logger = logger;
    }

    /// <summary>Вход по логину и паролю. При успехе ставится cookie сессии.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> Login(LoginRequest request)
    {
        var user = _users.GetByLogin(request.Login.Trim());

        // Проверяем хэш, а не сам пароль: открытый пароль нигде не хранится.
        var verified = user is not null &&
            _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
                != PasswordVerificationResult.Failed;

        if (user is null || !verified)
        {
            // В лог не пишем ни пароль, ни хэш.
            _logger.LogWarning("Login failed; login={Login}", request.Login);
            return Unauthorized(new ApiError("unauthorized", "Неверный логин или пароль"));
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.Role, user.Role),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        _logger.LogInformation("Login ok; userId={UserId}; role={Role}", user.Id, user.Role);
        return Ok(UserResponse.From(user));
    }

    /// <summary>Выход: cookie сессии удаляется.</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    /// <summary>Текущий пользователь. Без входа — 401.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    public ActionResult<UserResponse> Me()
    {
        var id = CurrentUser.GetId(User);
        var user = id is null ? null : _users.GetById(id.Value);

        if (user is null)
        {
            return Unauthorized(new ApiError("unauthorized", "Сессия не найдена"));
        }

        return Ok(UserResponse.From(user));
    }
}
