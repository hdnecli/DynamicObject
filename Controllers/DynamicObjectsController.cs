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

    [HttpPost]
    public async Task<ActionResult<DynamicObject>> CreateDynamicObject(DynamicObject dynamicObject)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.DynamicObjects.Add(dynamicObject);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDynamicObject), new { id = dynamicObject.Id }, dynamicObject);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DynamicObject>> GetDynamicObject(int id)
    {
        var dynamicObject = await _context.DynamicObjects.FindAsync(id);
        if (dynamicObject == null)
        {
            return NotFound();
        }
        return dynamicObject;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDynamicObject(int id, DynamicObject dynamicObject)
    {
        if (id != dynamicObject.Id)
        {
            return BadRequest();
        }

        _context.Entry(dynamicObject).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DynamicObjectExists(id))
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
    public async Task<IActionResult> DeleteDynamicObject(int id)
    {
        var dynamicObject = await _context.DynamicObjects.FindAsync(id);
        if (dynamicObject == null)
        {
            return NotFound();
        }

        _context.DynamicObjects.Remove(dynamicObject);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DynamicObjectExists(int id)
    {
        return _context.DynamicObjects.Any(e => e.Id == id);
    }
}
