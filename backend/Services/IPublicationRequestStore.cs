using KbKgeu.Api.Dtos;
using KbKgeu.Api.Models;

namespace KbKgeu.Api.Services;

/// <summary>
/// Хранилище заявок. Сервис зависит от интерфейса, а не от конкретной реализации:
/// в ЛР8 это список в памяти, в ЛР10 — PostgreSQL через EF Core.
/// </summary>
public interface IPublicationRequestStore
{
    IReadOnlyList<PublicationRequest> GetAll();

    /// <summary>
    /// Список для экрана: отбор выполняется в базе, а не в памяти.
    /// Пустые параметры означают «без этого условия».
    /// </summary>
    /// <param name="createdByUserId">Только заявки этого автора (ФТ-4).</param>
    /// <param name="assigneeUserId">Только заявки, назначенные этому редактору (ФТ-8).</param>
    /// <param name="status">Фильтр по статусу (ФТ-5).</param>
    /// <param name="departmentId">Фильтр по подразделению (ФТ-5).</param>
    IReadOnlyList<PublicationRequestResponse> GetList(
        int? createdByUserId,
        int? assigneeUserId,
        string? status,
        int? departmentId);

    PublicationRequest? GetById(int id);

    /// <summary>Сохранить новую заявку. Реализация присваивает Id.</summary>
    PublicationRequest Add(PublicationRequest item);

    /// <summary>Сохранить изменения существующей заявки.</summary>
    void Update(PublicationRequest item);
}
