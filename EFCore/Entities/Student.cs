using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Entities;
[Index(nameof(StudentName), IsUnique = true)]
public class Student
{
    public int StudentId { get; set; }
    [ConcurrencyCheck]
    public string StudentName { get; set; }
}