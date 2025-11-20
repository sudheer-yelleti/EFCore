using Microsoft.EntityFrameworkCore;
using EFCore.Entities;
namespace EFCore.DBContext;
public class StudentDbContext : DbContext
{
    public StudentDbContext(DbContextOptions<StudentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
    
    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<Student>().ToTable("Students");
    //     modelBuilder.Entity<Course>().ToTable("Courses");
    //     modelBuilder.Entity<CourseEnrollment>().ToTable("CourseEnrollments");
    // }
}