using System.ComponentModel.DataAnnotations;

namespace KbKgeu.Api.Dtos;

/// <summary>Смена статуса — отдельное действие, отдельный запрос.</summary>
public class ChangeStatusRequest
{
    [Required(ErrorMessage = "Статус обязателен")]
    [RegularExpression("^(New|InProgress|Closed|Cancelled)$",
        ErrorMessage = "Статус: New, InProgress, Closed или Cancelled")]
    public string Status { get; set; } = string.Empty;
}