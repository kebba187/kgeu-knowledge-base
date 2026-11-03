using System.Text.Json.Serialization;

namespace KbKgeu.Api.Dtos;

/// <summary>
/// Единый формат отказа из docs/api-contract.md.
/// Клиент получает только понятный текст: ни stack trace, ни путей, ни тела запроса.
/// </summary>
public class ApiError
{
    public ApiError(string error, string message, IDictionary<string, string>? fields = null)
    {
        Error = error;
        Message = message;
        Fields = fields;
    }

    public string Error { get; }

    public string Message { get; }

    /// <summary>Ошибки по полям. В ответе появляется только при проверке полей.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string>? Fields { get; }

    public static ApiError Validation(IDictionary<string, string> fields) =>
        new("validation", "Проверьте поля", fields);

    public static ApiError NotFound(string message) => new("not_found", message);

    public static ApiError Conflict(string message) => new("conflict", message);

    public static ApiError Server() =>
        new("server_error", "Сервер не смог обработать запрос");
}
