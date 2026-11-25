namespace KbKgeu.Api.Models;

/// <summary>
/// Заявка на публикацию материала в базе знаний КГЭУ — главный объект процесса.
/// Поля совпадают с docs/data-model.md.
/// </summary>
public class PublicationRequest
{
    /// <summary>Машинный ключ.</summary>
    public int Id { get; set; }

    /// <summary>Человеческий номер вида БЗ-2026-0001.</summary>
    public string Number { get; set; } = string.Empty;

    /// <summary>Название материала, 5–80 символов.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Что опубликовать и на каком основании, 10–500 символов.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Ссылка на справочник подразделений.</summary>
    public int DepartmentId { get; set; }

    /// <summary>
    /// Навигационное свойство к справочнику подразделений.
    /// Без него EF Core не знает, что DepartmentId ссылается на Department,
    /// и нельзя ни .Include, ни JOIN в проекции.
    /// </summary>
    public Department? Department { get; set; }

    /// <summary>New, InProgress, Closed или Cancelled.</summary>
    public string Status { get; set; } = RequestStatus.New;

    /// <summary>Автор заявки.</summary>
    public int CreatedByUserId { get; set; }

    /// <summary>Навигационное свойство к автору заявки.</summary>
    public User? CreatedByUser { get; set; }

    /// <summary>Назначенный редактор. У новой заявки редактора нет.</summary>
    public int? AssigneeUserId { get; set; }

    /// <summary>Навигационное свойство к назначенному редактору.</summary>
    public User? AssigneeUser { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
