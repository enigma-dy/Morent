using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoRent_V2.Context;
using MoRent_V2.Models;

namespace MoRent_V2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SponsorAdController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SponsorAdController(ApplicationDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<SponsorAd>>> GetSponsorAds()
    {
        return await _context.SponsorAds
            .Include(sa => sa.Car)
            .ToListAsync();
    }

    // GET: api/SponsorAd/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SponsorAd>> GetSponsorAd(int id)
    {
        var sponsorAd = await _context.SponsorAds
            .Include(sa => sa.Car) // Include the associated Car
            .FirstOrDefaultAsync(sa => sa.Id == id);

        if (sponsorAd == null)
        {
            return NotFound();
        }

        return sponsorAd;
    }

    [HttpPost]
    public async Task<ActionResult<SponsorAd>> PostSponsorAd(SponsorAd sponsorAd)
    {

        Console.WriteLine($"Received SponsorAd: {JsonSerializer.Serialize(sponsorAd)}");

        var car = await _context.Cars.FindAsync(sponsorAd.CarId);
        if (car == null)
        {
            return BadRequest("The specified Car does not exist.");
        }

        _context.SponsorAds.Add(sponsorAd);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSponsorAd", new { id = sponsorAd.Id }, sponsorAd);
    }

    // PUT: api/SponsorAd/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSponsorAd(int id, SponsorAd sponsorAd)
    {
        if (id != sponsorAd.Id)
        {
            return BadRequest();
        }

        // Validate that the Car exists
        var car = await _context.Cars.FindAsync(sponsorAd.CarId);
        if (car == null)
        {
            return BadRequest("The specified Car does not exist.");
        }

        _context.Entry(sponsorAd).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SponsorAdExists(id))
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

    // DELETE: api/SponsorAd/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSponsorAd(int id)
    {
        var sponsorAd = await _context.SponsorAds.FindAsync(id);
        if (sponsorAd == null)
        {
            return NotFound();
        }

        _context.SponsorAds.Remove(sponsorAd);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SponsorAdExists(int id)
    {
        return _context.SponsorAds.Any(e => e.Id == id);
    }
}