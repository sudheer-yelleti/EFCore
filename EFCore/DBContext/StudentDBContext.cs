//using EFCore.Entities;
using Microsoft.EntityFrameworkCore;

public class StudentDBContext : DbContext
{
    public StudentDBContext(DbContextOptions<StudentDBContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
}