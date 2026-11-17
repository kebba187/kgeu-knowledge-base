using KbKgeu.Api.Models;

namespace KbKgeu.Api.Services;

/// <summary>Доступ к пользователям: вход и проверка прав.</summary>
public interface IUserStore
{
    User? GetByLogin(string login);

    User? GetById(int id);

    IReadOnlyList<User> GetByRole(string role);
}
