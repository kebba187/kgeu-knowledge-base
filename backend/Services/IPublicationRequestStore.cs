using KbKgeu.Api.Models;

namespace KbKgeu.Api.Services;

/// <summary>
/// Хранилище заявок. Контроллер зависит от интерфейса, а не от конкретной реализации:
/// в ЛР8 это список в памяти, в ЛР10 — PostgreSQL через EF Core.
/// </summary>
public interface IPublicationRequestStore
{
    IReadOnlyList<PublicationRequest> GetAll();

    PublicationRequest? GetById(int id);
}
