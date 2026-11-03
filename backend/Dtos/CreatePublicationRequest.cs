using System.ComponentModel.DataAnnotations;

namespace KbKgeu.Api.Dtos;

/// <summary>
/// Что присылает клиент при создании. Поля id, number, status и исполнителя
/// задаёт сервер, поэтому их здесь нет.
/// </summary>
public class CreatePublicationRequest
{
    [Required(ErrorMessage = "Заголовок обязателен")]
    [StringLength(80, MinimumLength = 5, ErrorMessage = "От 5 до 80 символов")]
    public string Title { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Целое число больше нуля")]
    public int DepartmentId { get; set; }

    [Required(ErrorMessage = "Описание обязательно")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "От 10 до 500 символов")]
    public string Description { get; set; } = string.Empty;
}
