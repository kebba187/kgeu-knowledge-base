using KbKgeu.Api.Dtos;
using KbKgeu.Api.Models;

namespace KbKgeu.Api.Services;

/// <summary>
/// Правила процесса согласования материалов.
/// Контроллер принимает HTTP и переводит результат в коды ответа,
/// а решения «можно / нельзя» принимаются здесь.
/// </summary>
public class PublicationRequestService
{
    private readonly IPublicationRequestStore _store;
    private readonly ILogger<PublicationRequestService> _logger;

    public PublicationRequestService(
        IPublicationRequestStore store,
        ILogger<PublicationRequestService> logger)
    {
        _store = store;
        _logger = logger;
    }

    public IReadOnlyList<PublicationRequest> GetAll() => _store.GetAll();

    public PublicationRequest? GetById(int id) => _store.GetById(id);

    /// <summary>
    /// Создать заявку. Статус всегда New, редактор не назначен,
    /// номер и id задаёт сервер.
    /// </summary>
    public PublicationRequest Create(CreatePublicationRequest request, int authorUserId)
    {
        var now = DateTimeOffset.UtcNow;
        var item = new PublicationRequest
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            DepartmentId = request.DepartmentId,
            Status = RequestStatus.New,
            CreatedByUserId = authorUserId,
            AssigneeUserId = null,
            CreatedAt = now,
            UpdatedAt = now,
        };

        var created = _store.Add(item);
        created.Number = $"БЗ-{now.Year}-{created.Id:0000}";
        _store.Update(created);

        _logger.LogInformation(
            "CreatePublicationRequest ok; id={Id}; number={Number}; departmentId={DepartmentId}",
            created.Id,
            created.Number,
            created.DepartmentId);

        return created;
    }

    /// <summary>Назначить редактора. Статус при этом не меняется.</summary>
    public OperationResult<PublicationRequest> AssignEditor(int id, int editorUserId)
    {
        var item = _store.GetById(id);
        if (item is null)
        {
            return OperationResult<PublicationRequest>.NotFound($"Заявка {id} не найдена");
        }

        if (item.Status is RequestStatus.Closed or RequestStatus.Cancelled)
        {
            return OperationResult<PublicationRequest>.Conflict(
                "Нельзя менять редактора у опубликованной или отклонённой заявки");
        }

        item.AssigneeUserId = editorUserId;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        _store.Update(item);

        return OperationResult<PublicationRequest>.Success(item);
    }

    /// <summary>Сменить статус по правилам процесса.</summary>
    public OperationResult<PublicationRequest> ChangeStatus(int id, string status)
    {
        if (!RequestStatus.IsKnown(status))
        {
            return OperationResult<PublicationRequest>.Invalid(
                $"Неизвестный статус. Допустимы: {string.Join(", ", RequestStatus.All)}");
        }

        var item = _store.GetById(id);
        if (item is null)
        {
            return OperationResult<PublicationRequest>.NotFound($"Заявка {id} не найдена");
        }

        if (!RequestStatus.CanMove(item.Status, status))
        {
            return OperationResult<PublicationRequest>.Conflict(
                $"Нельзя перевести заявку из {item.Status} в {status}");
        }

        // Правило процесса: в работу берёт назначенный редактор, значит он должен быть.
        if (status == RequestStatus.InProgress && item.AssigneeUserId is null)
        {
            return OperationResult<PublicationRequest>.Conflict(
                "Сначала назначьте редактора: без него заявку нельзя взять на проверку");
        }

        item.Status = status;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        _store.Update(item);

        return OperationResult<PublicationRequest>.Success(item);
    }
}
