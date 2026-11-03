using KbKgeu.Api.Dtos;
using KbKgeu.Api.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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
// и не создают реализацию через new. В ЛР10 хранилище сменится на EF Core.
builder.Services.AddSingleton<IPublicationRequestStore, InMemoryPublicationRequestStore>();
builder.Services.AddScoped<PublicationRequestService>();

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

app.MapControllers();

app.Run();
