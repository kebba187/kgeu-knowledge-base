using KbKgeu.Api.Models;

namespace KbKgeu.Api.Dtos;

/// <summary>Что видит клиент. Формат совпадает с docs/api-contract.md.</summary>
public class PublicationRequestResponse
{
    public int Id { get; set; }

    public string Number { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int DepartmentId { get; set; }

    /// <summary>
    /// Название подразделения из справочника. Клиент больше не хранит
    /// собственную копию списка подразделений (docs/known-defects.md, D-02).
    /// </summary>
    public string? DepartmentName { get; set; }

    public string Status { get; set; } = string.Empty;

    public int CreatedByUserId { get; set; }

    /// <summary>ФИО автора заявки.</summary>
    public string? CreatedByFullName { get; set; }

    public int? AssigneeUserId { get; set; }

    /// <summary>ФИО назначенного редактора. Null, пока редактор не назначен.</summary>
    public string? AssigneeFullName { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Преобразование одной заявки — для ответов на POST и PATCH.
    /// Имена заполняются, только если навигационные свойства загружены.
    /// Список строится проекцией в SQL, см. EfPublicationRequestStore.GetList.
    /// </summary>
    public static PublicationRequestResponse From(PublicationRequest item) => new()
    {
        Id = item.Id,
        Number = item.Number,
        Title = item.Title,
        Description = item.Description,
        DepartmentId = item.DepartmentId,
        DepartmentName = item.Department?.Name,
        Status = item.Status,
        CreatedByUserId = item.CreatedByUserId,
        CreatedByFullName = item.CreatedByUser?.FullName,
        AssigneeUserId = item.AssigneeUserId,
        AssigneeFullName = item.AssigneeUser?.FullName,
        CreatedAt = item.CreatedAt,
        UpdatedAt = item.UpdatedAt,
    };
}
