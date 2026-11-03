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

    public string Status { get; set; } = string.Empty;

    public int CreatedByUserId { get; set; }

    public int? AssigneeUserId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public static PublicationRequestResponse From(PublicationRequest item) => new()
    {
        Id = item.Id,
        Number = item.Number,
        Title = item.Title,
        Description = item.Description,
        DepartmentId = item.DepartmentId,
        Status = item.Status,
        CreatedByUserId = item.CreatedByUserId,
        AssigneeUserId = item.AssigneeUserId,
        CreatedAt = item.CreatedAt,
        UpdatedAt = item.UpdatedAt,
    };
}
