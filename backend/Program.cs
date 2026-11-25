using System.Text.Encodings.Web;
using System.Text.Json;
using KbKgeu.Api.Data;
using KbKgeu.Api.Dtos;
using KbKgeu.Api.Models;
using KbKgeu.Api.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// Русский текст в ответе не должен превращаться в \u04xx.
var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
};

Task WriteApiError(HttpResponse response, int statusCode, ApiError error)
{
    response.StatusCode = statusCode;
    response.ContentType = "application/json";
    return response.WriteAsync(JsonSerializer.Serialize(error, jsonOptions));
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ошибки проверки полей отдаём в формате docs/api-contract.md, а не стандартным ProblemDetails.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var fields = context.ModelState
            .Where(entry => entry.Value is not null && entry.Value.Errors.Count > 0)
            .ToDictionary(
                entry => char.ToLowerInvariant(entry.Key[0]) + entry.Key[1..],
                entry => entry.Value!.Errors[0].ErrorMessage);

        return new BadRequestObjectResult(ApiError.Validation(fields));
    };
});

// Регистрация через DI: сервис и контроллер получают зависимости в конструкторе
// и не создают реализацию через new. Хранилище — PostgreSQL через EF Core.
builder.Services.AddPostgresStorage(builder.Configuration);
builder.Services.AddScoped<PublicationRequestService>();

// Хэширование паролей библиотекой ASP.NET Core Identity: соль и стоимость внутри.
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

// Вход по cookie: сервер не отдаёт пароль обратно и не хранит его в JavaScript.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "kb.session";
        options.Cookie.HttpOnly = true;
        // Клиент на другом порту (5173), поэтому cookie межсайтовая.
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;

        // API отвечает кодами, а не перенаправлением на страницу входа.
        options.Events.OnRedirectToLogin = context => WriteApiError(
            context.Response,
            StatusCodes.Status401Unauthorized,
            new ApiError("unauthorized", "Нужно войти в систему"));

        options.Events.OnRedirectToAccessDenied = context => WriteApiError(
            context.Response,
            StatusCodes.Status403Forbidden,
            new ApiError("forbidden", "Недостаточно прав для этого действия"));
    });

builder.Services.AddControllers(options =>
{
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
        (value, field) => $"Значение «{value}» не подходит для поля {field}");
    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
        value => $"Значение «{value}» не подходит");
});

builder.Services.AddAuthorization();

// CORS: разрешаем только клиент курса и только с передачей cookie.
const string ClientCorsPolicy = "client";
var clientOrigin = builder.Configuration["Cors:ClientOrigin"] ?? "http://localhost:5173";
builder.Services.AddCors(options => options.AddPolicy(ClientCorsPolicy, policy => policy
    .WithOrigins(clientOrigin)
    .AllowCredentials()
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

// Неожиданный сбой: в лог — одна понятная строка, клиенту — безопасный ответ.
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var feature = context.Features.Get<IExceptionHandlerFeature>();
    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
        .CreateLogger("UnhandledException");

    logger.LogError(
        "{Method} {Path} failed; error={ErrorType}; message={ErrorMessage}",
        context.Request.Method,
        context.Request.Path.Value,
        feature?.Error.GetType().Name,
        feature?.Error.Message);

    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsJsonAsync(ApiError.Server());
}));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(ClientCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Учебные пользователи и подразделения: только в Development.
app.SeedStudyData();

app.Run();
