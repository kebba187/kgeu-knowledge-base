using KbKgeu.Api.Dtos;
using KbKgeu.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KbKgeu.Api.Controllers;

/// <summary>
/// Заявки на публикацию материалов базы знаний.
/// Контроллер принимает HTTP и переводит результат сервиса в коды ответа.
/// DELETE в курсе не используется: отмена — это статус Cancelled.
/// </summary>
[ApiController]
[Route("api/publication-requests")]
[Produces("application/json")]
public class PublicationRequestsController : ControllerBase
{
    // До ЛР11 входа нет: автором считается учебный пользователь.
    private const int DemoAuthorUserId = 1;

    private readonly PublicationRequestService _service;

    public PublicationRequestsController(PublicationRequestService service)
    {
        _service = service;
    }

    /// <summary>Список заявок. Пустой список — это 200 и [], а не 404.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PublicationRequestResponse>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<PublicationRequestResponse>> GetAll()
    {
        var items = _service.GetAll().Select(PublicationRequestResponse.From);
        return Ok(items);
    }

    /// <summary>Одна заявка по машинному id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PublicationRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public ActionResult<PublicationRequestResponse> GetById(int id)
    {
        var item = _service.GetById(id);
        if (item is null)
        {
            return NotFound(ApiError.NotFound($"Заявка {id} не найдена"));
        }

        return Ok(PublicationRequestResponse.From(item));
    }

    /// <summary>Создать заявку: ответ 201, статус New, номер от сервера.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PublicationRequestResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public ActionResult<PublicationRequestResponse> Create(CreatePublicationRequest request)
    {
        var created = _service.Create(request, DemoAuthorUserId);
        var response = PublicationRequestResponse.From(created);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Назначить редактора — отдельное действие от смены статуса.</summary>
    [HttpPatch("{id:int}/assignee")]
    [ProducesResponseType(typeof(PublicationRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    public ActionResult<PublicationRequestResponse> AssignEditor(int id, AssignEditorRequest request)
    {
        var result = _service.AssignEditor(id, request.AssigneeUserId);
        return FromResult(result);
    }

    /// <summary>Сменить статус — отдельное действие от назначения.</summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(PublicationRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    public ActionResult<PublicationRequestResponse> ChangeStatus(int id, ChangeStatusRequest request)
    {
        var result = _service.ChangeStatus(id, request.Status);
        return FromResult(result);
    }

    private ActionResult<PublicationRequestResponse> FromResult(
        OperationResult<Models.PublicationRequest> result)
    {
        switch (result.Status)
        {
            case OperationStatus.Success:
                return Ok(PublicationRequestResponse.From(result.Value!));

            case OperationStatus.NotFound:
                return NotFound(ApiError.NotFound(result.Message!));

            case OperationStatus.Conflict:
                return Conflict(ApiError.Conflict(result.Message!));

            default:
                return BadRequest(ApiError.Validation(
                    new Dictionary<string, string> { ["status"] = result.Message! }));
        }
    }
}
