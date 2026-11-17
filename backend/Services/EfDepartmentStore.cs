using KbKgeu.Api.Data;
using KbKgeu.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace KbKgeu.Api.Services;

/// <summary>Справочник подразделений из PostgreSQL.</summary>
public class EfDepartmentStore : IDepartmentStore
{
    private readonly AppDbContext _db;

    public EfDepartmentStore(AppDbContext db)
    {
        _db = db;
    }

    public IReadOnlyList<Department> GetAll() =>
        _db.Departments.AsNoTracking().OrderBy(item => item.Id).ToList();

    public bool Exists(int id) => _db.Departments.Any(item => item.Id == id);
}
