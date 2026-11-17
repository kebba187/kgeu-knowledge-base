using System.ComponentModel.DataAnnotations;
using KbKgeu.Api.Models;

namespace KbKgeu.Api.Dtos;

/// <summary>Вход: логин и пароль.</summary>
public class LoginRequest
{
    [Required(ErrorMessage = "Логин обязателен")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>Кто вошёл. Хэш пароля наружу не отдаётся никогда.</summary>
public class UserResponse
{
    public int Id { get; set; }

    public string Login { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public static UserResponse From(User user) => new()
    {
        Id = user.Id,
        Login = user.Login,
        FullName = user.FullName,
        Role = user.Role,
    };
}
