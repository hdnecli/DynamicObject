using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 

[Route("api/[controller]")]
[ApiController]
public class DynamicObjectsController : ControllerBase
{
    private readonly DynamicObjectContext _context;

    public DynamicObjectsController(DynamicObjectContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateObject(Object obj)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Objects.Add(obj);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetObject), new { id = obj.Id }, obj);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Object>> GetObject(int id)
    {
        var obj = await _context.Objects
            .Include(o => o.Fields)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (obj == null)
        {
            return NotFound();
        }

        return obj;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateObject(int id, Object obj) 
    {
        if (id != obj.Id)
        {
            return BadRequest();
        }

        var existingObject = await _context.Objects
            .Include(o => o.Fields) 
            .FirstOrDefaultAsync(o => o.Id == id);

        if (existingObject == null)
        {
            return NotFound();
        }

        existingObject.Name = obj.Name;

        if (existingObject.Fields != null){
            existingObject.Fields.Clear(); 
        
            foreach (var field in obj.Fields ?? Enumerable.Empty<Field>())
            {
                existingObject.Fields.Add(new Field
                {
                    FieldName = field.FieldName,
                    FieldValue = field.FieldValue
                });
            }
        }
        _context.Entry(existingObject).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ObjectExists(id)) 
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
    public async Task<IActionResult> DeleteObject(int id) 
    {
        var obj = await _context.Objects.FindAsync(id); 
        if (obj == null)
        {
            return NotFound();
        }

        _context.Objects.Remove(obj);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ObjectExists(int id) 
    {
        return _context.Objects.Any(e => e.Id == id);
    }

    [HttpPost("transaction")]
    public async Task<IActionResult> CreateObjects(List<Object> objects) 
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var obj in objects)
            {
                _context.Objects.Add(obj); 
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            return StatusCode(500, "An issue occurred");
        }

        return Ok(objects);
    }
}
