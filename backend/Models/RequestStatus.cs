namespace KbKgeu.Api.Models;

/// <summary>
/// Четыре статуса курса. Других значений в API и БД быть не должно.
/// </summary>
public static class RequestStatus
{
    public const string New = "New";
    public const string InProgress = "InProgress";
    public const string Closed = "Closed";
    public const string Cancelled = "Cancelled";

    public static readonly string[] All = { New, InProgress, Closed, Cancelled };

    public static bool IsKnown(string? status) => Array.IndexOf(All, status) >= 0;

    /// <summary>
    /// Допустимые переходы: New → InProgress / Cancelled, InProgress → Closed / Cancelled.
    /// Closed и Cancelled — конечные.
    /// </summary>
    public static bool CanMove(string from, string to) => (from, to) switch
    {
        (New, InProgress) => true,
        (New, Cancelled) => true,
        (InProgress, Closed) => true,
        (InProgress, Cancelled) => true,
        _ => false,
    };
}
