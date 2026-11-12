using KbKgeu.Api.Data;
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

    public PublicationRequest? GetById(int id) =>
        _db.PublicationRequests.FirstOrDefault(item => item.Id == id);

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
