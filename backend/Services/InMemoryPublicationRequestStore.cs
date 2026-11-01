using KbKgeu.Api.Models;

namespace KbKgeu.Api.Services;

/// <summary>
/// Заготовка хранилища на ЛР7: список в памяти, пока пустой.
/// Данные живут до перезапуска процесса — настоящее хранение появится в ЛР10.
/// </summary>
public class InMemoryPublicationRequestStore : IPublicationRequestStore
{
    private readonly List<PublicationRequest> _items = new();

    public IReadOnlyList<PublicationRequest> GetAll() => _items.ToList();

    public PublicationRequest? GetById(int id) => _items.FirstOrDefault(item => item.Id == id);
}
