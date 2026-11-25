using KbKgeu.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace KbKgeu.Api.Services;

/// <summary>
/// Учебные пользователи для режима хранения «в памяти» (Storage:Mode = Memory).
/// Пароли сразу хэшируются: открытого пароля в памяти не остаётся.
/// </summary>
public class InMemoryUserStore : IUserStore
{
    private readonly List<User> _users;

    public InMemoryUserStore(IPasswordHasher<User> hasher)
    {
        _users = StudyUsers.Accounts
        
            .Select((account, index) =>
            {
                var user = new User
                {
                    Id = index + 1,
                    Login = account.Login,
                    FullName = account.FullName,
                    Role = account.Role,
                };
                user.PasswordHash = hasher.HashPassword(user, account.Password);
                return user;
            })
            .ToList();
    }

    public User? GetByLogin(string login) =>
        _users.FirstOrDefault(user => string.Equals(user.Login, login, StringComparison.OrdinalIgnoreCase));

    public User? GetById(int id) => _users.FirstOrDefault(user => user.Id == id);

    public IReadOnlyList<User> GetByRole(string role) =>
        _users.Where(user => user.Role == role).ToList();
}

/// <summary>
/// Учебные учётные записи. Используются только на локальном стенде курса
/// и перечислены в README.
/// </summary>
public static class StudyUsers
{
    public record Account(string Login, string FullName, string Role, string Password);

    public static readonly Account[] Accounts =
    {
        new("author", "Сотрудник учебного отдела", UserRole.Author, "Author-2026"),
        new("coordinator", "Координатор базы знаний", UserRole.Coordinator, "Coordinator-2026"),
        new("editor", "Редактор базы знаний", UserRole.Editor, "Editor-2026"),
        new("editor2", "Второй редактор базы знаний", UserRole.Editor, "Editor2-2026"),
    };
}
