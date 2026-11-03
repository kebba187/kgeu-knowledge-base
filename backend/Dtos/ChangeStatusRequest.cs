using System.ComponentModel.DataAnnotations;

namespace KbKgeu.Api.Dtos;

/// <summary>Смена статуса — отдельное действие, отдельный запрос.</summary>
public class ChangeStatusRequest
{
    [Required(ErrorMessage = "Статус обязателен")]
    public string Status { get; set; } = string.Empty;
}
