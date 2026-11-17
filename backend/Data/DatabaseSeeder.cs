using KbKgeu.Api.Models;
using KbKgeu.Api.Services;
using Microsoft.AspNetCore.Identity;

namespace KbKgeu.Api.Data;

/// <summary>
/// Учебные пользователи стенда. Пароли не хранятся: в базу попадает только хэш.
/// Запускается в режиме Development, чтобы стенд поднимался по README без ручного SQL.
/// </summary>
public static class DatabaseSeeder
{
    public static void SeedStudyData(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(DatabaseSeeder));

        if (!db.Departments.Any())
        {
            db.Departments.AddRange(
                new Department { Name = "Учебный отдел" },
                new Department { Name = "Деканат" },
                new Department { Name = "Студенческий городок" },
                new Department { Name = "ИТ-служба" });
            db.SaveChanges();
            logger.LogInformation("Seed: добавлены подразделения");
        }

        foreach (var account in StudyUsers.Accounts)
        {
            var user = db.Users.FirstOrDefault(item => item.Login == account.Login);

            if (user is null)
            {
                user = new User
                {
                    Login = account.Login,
                    FullName = account.FullName,
                    Role = account.Role,
                };
                user.PasswordHash = hasher.HashPassword(user, account.Password);
                db.Users.Add(user);
                logger.LogInformation("Seed: создан пользователь {Login}", account.Login);
                continue;
            }

            // Заглушка из db/seed.sql заменяется настоящим хэшем.
            var verified = hasher.VerifyHashedPassword(user, user.PasswordHash, account.Password);
            if (verified == PasswordVerificationResult.Failed)
            {
                user.PasswordHash = hasher.HashPassword(user, account.Password);
                logger.LogInformation("Seed: обновлён хэш пароля {Login}", account.Login);
            }
        }

        db.SaveChanges();
    }
}
