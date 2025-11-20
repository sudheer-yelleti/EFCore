using EFCore.DBContext;
using EFCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseEnrollmentController:ControllerBase
{
    
    private readonly StudentDbContext _context;
    public CourseEnrollmentController(StudentDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseEnrollment>>> GetCourseEnrollments()
    {
        return await _context.CourseEnrollments.AsNoTracking().ToListAsync();
    }
    
    [HttpPost]
    public async Task<ActionResult<Course>> PostCourse(CourseEnrollment enrollment)
    {
        _context.CourseEnrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetCourseEnrollment), new { id = enrollment.CourseId }, enrollment);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CourseEnrollment>> GetCourseEnrollment(int id)
    {
        //var enrollment = await _context.CourseEnrollments.FindAsync(id);
        var enrollment = await _context.CourseEnrollments.FromSqlRaw("SELECT * FROM CourseEnrollments WHERE CourseId = {0}", id).FirstOrDefaultAsync();
        if (enrollment == null) return NotFound();
        return enrollment;
    }
    
}