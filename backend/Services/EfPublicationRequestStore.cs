using KbKgeu.Api.Data;
using KbKgeu.Api.Dtos;
using KbKgeu.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace KbKgeu.Api.Services;

/// <summary>
/// Хранилище заявок в PostgreSQL через EF Core.
/// Тот же интерфейс, что и у списка в памяти, поэтому сервис и контроллер не меняются.
/// </summary>
public class EfPublicationRequestStore : IPublicationRequestStore
{
    private readonly AppDbContext _db;

    public EfPublicationRequestStore(AppDbContext db)
    {
        _db = db;
    }

    public IReadOnlyList<PublicationRequest> GetAll() =>
        _db.PublicationRequests.AsNoTracking().OrderBy(item => item.Id).ToList();

    /// <summary>
    /// Отбор и сборка ответа выполняются одним запросом в базу:
    /// WHERE отсекает лишние строки, JOIN подставляет названия,
    /// SELECT возвращает только нужные колонки.
    /// </summary>
    public IReadOnlyList<PublicationRequestResponse> GetList(
        int? createdByUserId,
        int? assigneeUserId,
        string? status,
        int? departmentId)
    {
        var query = _db.PublicationRequests.AsNoTracking();

        if (createdByUserId is not null)
        {
            query = query.Where(item => item.CreatedByUserId == createdByUserId.Value);
        }

        if (assigneeUserId is not null)
        {
            query = query.Where(item => item.AssigneeUserId == assigneeUserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(item => item.Status == status);
        }

        if (departmentId is not null)
        {
            query = query.Where(item => item.DepartmentId == departmentId.Value);
        }

        return query
            .OrderByDescending(item => item.CreatedAt)
            .Select(item => new PublicationRequestResponse
            {
                Id = item.Id,
                Number = item.Number,
                Title = item.Title,
                Description = item.Description,
                DepartmentId = item.DepartmentId,
                DepartmentName = item.Department!.Name,
                Status = item.Status,
                CreatedByUserId = item.CreatedByUserId,
                CreatedByFullName = item.CreatedByUser!.FullName,
                AssigneeUserId = item.AssigneeUserId,
                AssigneeFullName = item.AssigneeUser!.FullName,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
            })
            .ToList();
    }

    // Навигационные свойства нужны, чтобы в ответе на POST и PATCH
    // тоже были названия подразделения и ФИО.
    public PublicationRequest? GetById(int id) =>
        _db.PublicationRequests
            .Include(item => item.Department)
            .Include(item => item.CreatedByUser)
            .Include(item => item.AssigneeUser)
            .FirstOrDefault(item => item.Id == id);

    public PublicationRequest Add(PublicationRequest item)
    {
        _db.PublicationRequests.Add(item);
        _db.SaveChanges();
        return item;
    }

    public void Update(PublicationRequest item)
    {
        _db.PublicationRequests.Update(item);
        _db.SaveChanges();
    }
}
