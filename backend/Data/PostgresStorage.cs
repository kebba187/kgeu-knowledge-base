using KbKgeu.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace KbKgeu.Api.Data;

/// <summary>
/// Регистрация PostgreSQL как хранилища. Строка подключения читается из конфигурации
/// (user-secrets или переменная окружения), в репозитории её нет.
/// </summary>
public static class PostgresStorage
{
    public static IServiceCollection AddPostgresStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Не задана строка подключения ConnectionStrings:Default. " +
                "Задайте её через dotnet user-secrets (см. README).");
        }

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IPublicationRequestStore, EfPublicationRequestStore>();

        return services;
    }
}
