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
    private readonly IUserStore _users;
    private readonly IDepartmentStore _departments;
    private readonly ILogger<PublicationRequestService> _logger;

    public PublicationRequestService(
        IPublicationRequestStore store,
        IUserStore users,
        IDepartmentStore departments,
        ILogger<PublicationRequestService> logger)
    {
        _store = store;
        _users = users;
        _departments = departments;
        _logger = logger;
    }

    public IReadOnlyList<PublicationRequest> GetAll() => _store.GetAll();

    public PublicationRequest? GetById(int id) => _store.GetById(id);

    /// <summary>
    /// Создать заявку. Статус всегда New, редактор не назначен,
    /// номер и id задаёт сервер.
    /// </summary>
    public OperationResult<PublicationRequest> Create(CreatePublicationRequest request, int authorUserId)
    {
        // Проверяем справочник сами: иначе внешний ключ базы дал бы 500 вместо понятного 400.
        if (!_departments.Exists(request.DepartmentId))
        {
            return OperationResult<PublicationRequest>.Invalid(
                "Такого подразделения нет в справочнике", "departmentId");
        }

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

        return OperationResult<PublicationRequest>.Success(created);
    }

    /// <summary>Назначить редактора. Статус при этом не меняется.</summary>
    public OperationResult<PublicationRequest> AssignEditor(int id, int editorUserId)
    {
        var item = _store.GetById(id);
        if (item is null)
        {
            return OperationResult<PublicationRequest>.NotFound($"Заявка {id} не найдена");
        }

        var editor = _users.GetById(editorUserId);
        if (editor is null || editor.Role != UserRole.Editor)
        {
            return OperationResult<PublicationRequest>.Invalid(
                "Назначить можно только пользователя с ролью редактора", "assigneeUserId");
        }

        if (item.Status is RequestStatus.Closed or RequestStatus.Cancelled)
        {
            return OperationResult<PublicationRequest>.Conflict(
                "Нельзя менять редактора у опубликованной или отклонённой заявки");
        }

        item.AssigneeUserId = editorUserId;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        _store.Update(item);

        _logger.LogInformation(
            "AssignEditor ok; id={Id}; assigneeUserId={AssigneeUserId}", item.Id, editorUserId);

        return OperationResult<PublicationRequest>.Success(item);
    }

    /// <summary>
    /// Сменить статус. Двигать заявку может только назначенный на неё редактор:
    /// чужую запись трогать нельзя (403), правила перехода проверяются отдельно (409).
    /// </summary>
    public OperationResult<PublicationRequest> ChangeStatus(int id, string status, int actorUserId)
    {
        if (!RequestStatus.IsKnown(status))
        {
            return OperationResult<PublicationRequest>.Invalid(
                $"Неизвестный статус. Допустимы: {string.Join(", ", RequestStatus.All)}", "status");
        }

        var item = _store.GetById(id);
        if (item is null)
        {
            return OperationResult<PublicationRequest>.NotFound($"Заявка {id} не найдена");
        }

        if (item.AssigneeUserId is null)
        {
            return OperationResult<PublicationRequest>.Conflict(
                "Сначала назначьте редактора: без него заявку нельзя двигать");
        }

        if (item.AssigneeUserId != actorUserId)
        {
            return OperationResult<PublicationRequest>.Forbidden(
                "Статус меняет только редактор, назначенный на эту заявку");
        }

        if (!RequestStatus.CanMove(item.Status, status))
        {
            return OperationResult<PublicationRequest>.Conflict(
                $"Нельзя перевести заявку из {item.Status} в {status}");
        }

        item.Status = status;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        _store.Update(item);

        _logger.LogInformation(
            "ChangeStatus ok; id={Id}; status={Status}; actorUserId={ActorUserId}",
            item.Id,
            status,
            actorUserId);

        return OperationResult<PublicationRequest>.Success(item);
    }
}
