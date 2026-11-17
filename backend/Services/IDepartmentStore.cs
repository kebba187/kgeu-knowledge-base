using KbKgeu.Api.Models;

namespace KbKgeu.Api.Services;

/// <summary>Справочник подразделений.</summary>
public interface IDepartmentStore
{
    IReadOnlyList<Department> GetAll();

    bool Exists(int id);
}

/// <summary>Справочник для режима хранения «в памяти».</summary>
public class InMemoryDepartmentStore : IDepartmentStore
{
    private static readonly Department[] Items =
    {
        new() { Id = 1, Name = "Учебный отдел" },
        new() { Id = 2, Name = "Деканат" },
        new() { Id = 3, Name = "Студенческий городок" },
        new() { Id = 4, Name = "ИТ-служба" },
    };

    public IReadOnlyList<Department> GetAll() => Items;

    public bool Exists(int id) => Items.Any(item => item.Id == id);
}
