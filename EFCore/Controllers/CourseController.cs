using EFCore.DBContext;
using EFCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController:ControllerBase
{
    
    private readonly StudentDbContext _context;
    public CourseController(StudentDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
    {
        return await _context.Courses.AsNoTracking().ToListAsync();
    }
    
    [HttpPost]
    public async Task<ActionResult<Course>> PostCourse(Course course)
    {
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, course);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Course>> GetCourse(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();
        return course;
    }
    
}