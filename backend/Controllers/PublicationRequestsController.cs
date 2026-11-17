using KbKgeu.Api.Auth;
using KbKgeu.Api.Dtos;
using KbKgeu.Api.Models;
using KbKgeu.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KbKgeu.Api.Controllers;

/// <summary>
/// Заявки на публикацию материалов базы знаний.
/// Контроллер принимает HTTP и переводит результат сервиса в коды ответа.
/// Без входа — 401, без нужной роли — 403 (см. docs/role-matrix.md).
/// DELETE в курсе не используется: отмена — это статус Cancelled.
/// </summary>
[ApiController]
[Route("api/publication-requests")]
[Produces("application/json")]
[Authorize]
public class PublicationRequestsController : ControllerBase
{
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

    /// <summary>Создать заявку: доступно роли автора, ответ 201, статус New.</summary>
    [HttpPost]
    [Authorize(Roles = UserRole.Author)]
    [ProducesResponseType(typeof(PublicationRequestResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status403Forbidden)]
    public ActionResult<PublicationRequestResponse> Create(CreatePublicationRequest request)
    {
        var authorId = CurrentUser.GetId(User);
        if (authorId is null)
        {
            return Unauthorized(new ApiError("unauthorized", "Сессия не найдена"));
        }

        var result = _service.Create(request, authorId.Value);
        if (result.Status != OperationStatus.Success)
        {
            return FromResult(result);
        }

        var created = result.Value!;
        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            PublicationRequestResponse.From(created));
    }

    /// <summary>Назначить редактора: доступно координатору, статус не меняется.</summary>
    [HttpPatch("{id:int}/assignee")]
    [Authorize(Roles = UserRole.Coordinator)]
    [ProducesResponseType(typeof(PublicationRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    public ActionResult<PublicationRequestResponse> AssignEditor(int id, AssignEditorRequest request)
    {
        var result = _service.AssignEditor(id, request.AssigneeUserId);
        return FromResult(result);
    }

    /// <summary>
    /// Сменить статус: двигает только назначенный редактор.
    /// Чужая заявка — 403, запрещённый переход — 409.
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = UserRole.Editor)]
    [ProducesResponseType(typeof(PublicationRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    public ActionResult<PublicationRequestResponse> ChangeStatus(int id, ChangeStatusRequest request)
    {
        var actorId = CurrentUser.GetId(User);
        if (actorId is null)
        {
            return Unauthorized(new ApiError("unauthorized", "Сессия не найдена"));
        }

        var result = _service.ChangeStatus(id, request.Status, actorId.Value);
        return FromResult(result);
    }

    private ActionResult<PublicationRequestResponse> FromResult(
        OperationResult<PublicationRequest> result)
    {
        switch (result.Status)
        {
            case OperationStatus.Success:
                return Ok(PublicationRequestResponse.From(result.Value!));

            case OperationStatus.NotFound:
                return NotFound(ApiError.NotFound(result.Message!));

            case OperationStatus.Conflict:
                return Conflict(ApiError.Conflict(result.Message!));

            case OperationStatus.Forbidden:
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new ApiError("forbidden", result.Message!));

            default:
                return BadRequest(ApiError.Validation(
                    new Dictionary<string, string> { [result.Field ?? "request"] = result.Message! }));
        }
    }
}
