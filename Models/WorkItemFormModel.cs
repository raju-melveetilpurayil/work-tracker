using System.ComponentModel.DataAnnotations;

public class WorkItemFormModel : IValidatableObject
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(120, ErrorMessage = "Title can't exceed 120 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description can't exceed 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start time is required.")]
    public DateTime StartTime { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "End time is required.")]
    public DateTime EndTime { get; set; } = DateTime.Now.AddHours(1);

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (EndTime <= StartTime)
        {
            yield return new ValidationResult(
                "End time must be after the start time.",
                new[] { nameof(EndTime) });
        }
    }
}