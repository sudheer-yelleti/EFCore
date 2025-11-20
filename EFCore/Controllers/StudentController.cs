using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EFCore.DBContext;
using EFCore.Entities;

namespace EFCore.Controllers;
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class StudentController : ControllerBase
{
    private readonly StudentDbContext _context;

    public StudentController(StudentDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<IEnumerable<Student>>> GetStudentsAsync()
    {
        return await _context.Students.AsNoTracking().ToListAsync();
    }

    [HttpGet("{id}", Name = "GetStudentAsync")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<Student>> GetStudentAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
        {
            return NotFound();
        }

        return student;
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<Student>> PostStudentAsync([FromBody]Student student)
    {
        
        await using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        return CreatedAtRoute(nameof(GetStudentAsync), new { id = student.StudentId }, student);
    }

    [HttpPut("{id}")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> PutStudentAsync(int id, [FromBody] Student student)
    {
        if (id != student.StudentId)
        {
            return BadRequest();
        }

        var existingStudent = await _context.Students.FindAsync(id);
        if (existingStudent == null) return NotFound();
        existingStudent.StudentName = student.StudentName;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StudentExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> DeleteStudentAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> PatchStudentSimpleAsync(int id, [FromBody] Student studentPatch)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return NotFound();

        // Update only provided fields
        if (!string.IsNullOrEmpty(studentPatch.StudentName))
            student.StudentName = studentPatch.StudentName;

        await _context.SaveChangesAsync();

        return Ok(student);
    }


    private bool StudentExists(int id)
    {
        return _context.Students.Any(e => e.StudentId == id);
    }
}