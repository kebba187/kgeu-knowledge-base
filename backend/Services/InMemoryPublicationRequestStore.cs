using KbKgeu.Api.Dtos;
using KbKgeu.Api.Models;

namespace KbKgeu.Api.Services;

/// <summary>
/// Хранилище ЛР8: список в памяти процесса.
/// После перезапуска сервера данные пропадают — это ожидаемо до ЛР10.
/// </summary>
public class InMemoryPublicationRequestStore : IPublicationRequestStore
{
    private readonly List<PublicationRequest> _items = new();
    private readonly object _lock = new();
    private int _lastId;

    public IReadOnlyList<PublicationRequest> GetAll()
    {
        lock (_lock)
        {
            return _items.OrderBy(item => item.Id).ToList();
        }
    }

    /// <summary>
    /// Те же условия отбора, что и у варианта с базой, но в памяти.
    /// Названия подразделения и ФИО здесь недоступны: у этого хранилища
    /// нет справочников, поэтому они остаются null.
    /// </summary>
    public IReadOnlyList<PublicationRequestResponse> GetList(
        int? createdByUserId,
        int? assigneeUserId,
        string? status,
        int? departmentId)
    {
        lock (_lock)
        {
            IEnumerable<PublicationRequest> query = _items;

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
                .Select(PublicationRequestResponse.From)
                .ToList();
        }
    }

    public PublicationRequest? GetById(int id)
    {
        lock (_lock)
        {
            return _items.FirstOrDefault(item => item.Id == id);
        }
    }

    public PublicationRequest Add(PublicationRequest item)
    {
        lock (_lock)
        {
            item.Id = ++_lastId;
            _items.Add(item);
            return item;
        }
    }

    public void Update(PublicationRequest item)
    {
        // Объект уже лежит в списке по ссылке: отдельное сохранение не требуется.
    }
}
