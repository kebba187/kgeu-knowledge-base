using KbKgeu.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KbKgeu.Api.Controllers;

/// <summary>
/// Служебные адреса: проверка живости процесса и сведения о приложении.
/// Создание заявок здесь не выполняется — это ЛР8.
/// </summary>
[ApiController]
[Route("/")]
public class ServiceController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    // Хранилище приходит через конструктор (DI), контроллер не создаёт его через new.
    private readonly IPublicationRequestStore _store;

    public ServiceController(IWebHostEnvironment environment, IPublicationRequestStore store)
    {
        _environment = environment;
        _store = store;
    }

    /// <summary>
    /// Проверка живости процесса. Тело запроса не принимается:
    /// успешный ответ не доказывает, что создание заявки работает.
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health() => Ok("ok");

    /// <summary>Сведения о приложении.</summary>
    [HttpGet("info")]
    public IActionResult Info() => Ok(new
    {
        application = "База знаний КГЭУ · API",
        environment = _environment.EnvironmentName,
        version = "ЛР7",
        storage = "память процесса; PostgreSQL появится в ЛР10",
        requestsInMemory = _store.GetAll().Count,
    });
}
