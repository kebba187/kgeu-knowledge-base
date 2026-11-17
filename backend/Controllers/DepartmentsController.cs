using KbKgeu.Api.Models;
using KbKgeu.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KbKgeu.Api.Controllers;

/// <summary>Справочник подразделений для формы создания заявки.</summary>
[ApiController]
[Route("api/departments")]
[Produces("application/json")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentStore _departments;

    public DepartmentsController(IDepartmentStore departments)
    {
        _departments = departments;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Department>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Department>> GetAll() => Ok(_departments.GetAll());
}
