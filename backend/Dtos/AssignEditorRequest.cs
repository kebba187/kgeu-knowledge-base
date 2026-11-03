using System.ComponentModel.DataAnnotations;

namespace KbKgeu.Api.Dtos;

/// <summary>Назначение редактора — отдельное действие, отдельный запрос.</summary>
public class AssignEditorRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Целое число больше нуля")]
    public int AssigneeUserId { get; set; }
}
