using KbKgeu.Api.Models;

namespace KbKgeu.Api.Services;

/// <summary>
/// Хранилище заявок. Сервис зависит от интерфейса, а не от конкретной реализации:
/// в ЛР8 это список в памяти, в ЛР10 — PostgreSQL через EF Core.
/// </summary>
public interface IPublicationRequestStore
{
    IReadOnlyList<PublicationRequest> GetAll();

    PublicationRequest? GetById(int id);

    /// <summary>Сохранить новую заявку. Реализация присваивает Id.</summary>
    PublicationRequest Add(PublicationRequest item);

    /// <summary>Сохранить изменения существующей заявки.</summary>
    void Update(PublicationRequest item);
}
