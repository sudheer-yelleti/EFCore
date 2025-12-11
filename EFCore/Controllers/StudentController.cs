using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EFCore.DBContext;
using EFCore.Entities;
using EFCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;

namespace EFCore.Controllers;

[Authorize]
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class StudentController(StudentDbContext context, IDistributedCache cache, ILogger<StudentController> logger)
    : ControllerBase
{
    [HttpGet]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<IEnumerable<Student>>> GetStudentsAsync()
    {
        var cacheKey = "students";
        logger.LogInformation("Getting students from cache");
        var students = await cache.GetOrSetAsync(cacheKey,
            async () =>
            {
                logger.LogInformation("Cache Miss. Getting students from database");
                return await context.Students.AsNoTracking().ToListAsync();
            });
        // return await _context.Students.AsNoTracking().ToListAsync();
        return students;
    }

    [HttpGet("{id}", Name = "GetStudentAsync")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<Student>> GetStudentAsync(int id)
    {
        var student = await context.Students.FindAsync(id);

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
        
        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                context.Students.Add(student);
                await context.SaveChangesAsync();
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

        var existingStudent = await context.Students.FindAsync(id);
        if (existingStudent == null) return NotFound();
        existingStudent.StudentName = student.StudentName;

        try
        {
            await context.SaveChangesAsync();
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
        var student = await context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        context.Students.Remove(student);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> PatchStudentSimpleAsync(int id, [FromBody] Student studentPatch)
    {
        var student = await context.Students.FindAsync(id);
        if (student == null) return NotFound();

        // Update only provided fields
        if (!string.IsNullOrEmpty(studentPatch.StudentName))
            student.StudentName = studentPatch.StudentName;

        await context.SaveChangesAsync();

        return Ok(student);
    }


    private bool StudentExists(int id)
    {
        return context.Students.Any(e => e.StudentId == id);
    }
}