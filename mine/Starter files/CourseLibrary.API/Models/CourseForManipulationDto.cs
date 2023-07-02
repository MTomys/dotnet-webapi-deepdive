using System.ComponentModel.DataAnnotations;
using CourseLibrary.API.ValidationAttributes;

namespace CourseLibrary.API.Models;

[CourseTitleMustBeDifferentFromDescription]
public abstract class CourseForManipulationDto
{
    [Required(ErrorMessage = "No title found")]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1500, ErrorMessage = "Description too long")]
    public virtual string Description { get; set; } = string.Empty;
}