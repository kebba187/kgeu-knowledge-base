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
