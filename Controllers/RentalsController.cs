using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoRent_Server.Models;
using MoRent_V2.Context;

namespace MoRent_V2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RentalsController(ApplicationDbContext context) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;

    // GET: api/Rentals
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rental>>> GetRentals()
    {
        return await _context.Rentals.ToListAsync();
    }

    // GET: api/Rentals/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Rental>> GetRental(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);

        if (rental == null)
        {
            return NotFound();
        }

        return rental;
    }

    // PUT: api/Rentals/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRental(int id, Rental rental)
    {
        if (id != rental.Id)
        {
            return BadRequest();
        }

        _context.Entry(rental).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!RentalExists(id))
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

    // POST: api/Rentals
    [HttpPost]
    public async Task<ActionResult<Rental>> PostRental(Rental rental)
    {
        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetRental", new { id = rental.Id }, rental);
    }

    // DELETE: api/Rentals/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRental(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null)
        {
            return NotFound();
        }

        _context.Rentals.Remove(rental);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool RentalExists(int id)
    {
        return _context.Rentals.Any(e => e.Id == id);
    }
}
