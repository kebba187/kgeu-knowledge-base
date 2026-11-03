namespace KbKgeu.Api.Services;

/// <summary>Чем закончилось действие с точки зрения правил процесса.</summary>
public enum OperationStatus
{
    Success,
    NotFound,
    Conflict,
    Invalid,
}

/// <summary>
/// Результат операции сервиса. Контроллер переводит его в HTTP-код:
/// Success → 200, NotFound → 404, Conflict → 409, Invalid → 400.
/// </summary>
public class OperationResult<T>
{
    private OperationResult(OperationStatus status, T? value, string? message)
    {
        Status = status;
        Value = value;
        Message = message;
    }

    public OperationStatus Status { get; }

    public T? Value { get; }

    public string? Message { get; }

    public static OperationResult<T> Success(T value) => new(OperationStatus.Success, value, null);

    public static OperationResult<T> NotFound(string message) =>
        new(OperationStatus.NotFound, default, message);

    public static OperationResult<T> Conflict(string message) =>
        new(OperationStatus.Conflict, default, message);

    public static OperationResult<T> Invalid(string message) =>
        new(OperationStatus.Invalid, default, message);
}
