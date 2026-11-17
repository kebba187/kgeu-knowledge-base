using KbKgeu.Api.Data;
using KbKgeu.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace KbKgeu.Api.Services;

/// <summary>Пользователи из PostgreSQL через EF Core.</summary>
public class EfUserStore : IUserStore
{
    private readonly AppDbContext _db;

    public EfUserStore(AppDbContext db)
    {
        _db = db;
    }

    public User? GetByLogin(string login) =>
        _db.Users.AsNoTracking().FirstOrDefault(user => user.Login.ToLower() == login.ToLower());

    public User? GetById(int id) =>
        _db.Users.AsNoTracking().FirstOrDefault(user => user.Id == id);

    public IReadOnlyList<User> GetByRole(string role) =>
        _db.Users.AsNoTracking().Where(user => user.Role == role).ToList();
}
