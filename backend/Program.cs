using KbKgeu.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Регистрация через DI: контроллер получает IPublicationRequestStore в конструкторе
// и не создаёт реализацию через new. В ЛР10 здесь появится реализация на EF Core.
builder.Services.AddSingleton<IPublicationRequestStore, InMemoryPublicationRequestStore>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
