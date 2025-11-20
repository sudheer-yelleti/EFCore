using System.ComponentModel.DataAnnotations;

namespace EFCore.Entities;
public class Course
{
    public int CourseId { get; set; }
    [ConcurrencyCheck]
    public string CourseName { get; set; }
}