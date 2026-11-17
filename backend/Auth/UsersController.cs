using KbKgeu.Api.Dtos;
using KbKgeu.Api.Models;
using KbKgeu.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KbKgeu.Api.Auth;

/// <summary>
/// Список редакторов для экрана назначения. Доступен координатору:
/// обычному автору список пользователей не нужен.
/// </summary>
[ApiController]
[Route("api/users")]
[Produces("application/json")]
[Authorize(Roles = UserRole.Coordinator)]
public class UsersController : ControllerBase
{
    private readonly IUserStore _users;

    public UsersController(IUserStore users)
    {
        _users = users;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<UserResponse>> GetByRole([FromQuery] string role = UserRole.Editor)
    {
        if (Array.IndexOf(UserRole.All, role) < 0)
        {
            return BadRequest(ApiError.Validation(
                new Dictionary<string, string> { ["role"] = "Неизвестная роль" }));
        }

        return Ok(_users.GetByRole(role).Select(UserResponse.From));
    }
}
