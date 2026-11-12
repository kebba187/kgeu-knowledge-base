namespace KbKgeu.Api.Models;

/// <summary>Пользователь системы: таблица users.</summary>
public class User
{
    public int Id { get; set; }

    public string Login { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    /// <summary>Author, Coordinator или Editor.</summary>
    public string Role { get; set; } = UserRole.Author;

    /// <summary>Только хэш пароля. Открытый пароль нигде не хранится.</summary>
    public string PasswordHash { get; set; } = string.Empty;
}

/// <summary>Роли из ТЗ (docs/lab-01-requirements.md).</summary>
public static class UserRole
{
    public const string Author = "Author";
    public const string Coordinator = "Coordinator";
    public const string Editor = "Editor";

    public static readonly string[] All = { Author, Coordinator, Editor };
}
