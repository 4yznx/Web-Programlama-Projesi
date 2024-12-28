using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BarberShop.Models;

namespace BarberShop.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly BarberDbContext _context;

        public ApiController(BarberDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Islem>>> GetIslemler()
        {
            return await _context.Islemler.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Islem>> GetIslem(int id)
        {
            var islem = await _context.Islemler.FindAsync(id);

            if (islem == null)
            {
                return NotFound();
            }

            return islem;
        }

        [HttpPost]
        public async Task<ActionResult<Islem>> CreateIslem(Islem islem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Islemler.Add(islem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIslem), new { id = islem.IslemID }, islem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIslem(int id, Islem islem)
        {
            if (id != islem.IslemID)
            {
                return BadRequest();
            }

            _context.Entry(islem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IslemExists(id))
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
        public async Task<IActionResult> DeleteIslem(int id)
        {
            var islem = await _context.Islemler.FindAsync(id);

            if (islem == null)
            {
                return NotFound();
            }

            _context.Islemler.Remove(islem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IslemExists(int id)
        {
            return _context.Islemler.Any(e => e.IslemID == id);
        }

        [HttpGet("filter-by-price")]
        public async Task<ActionResult<IEnumerable<Islem>>> GetIslemlerByPrice([FromQuery] decimal maxPrice)
        {
            var filteredIslemler = await _context.Islemler
                .Where(i => i.Ucret < maxPrice)
                .ToListAsync();

            return Ok(filteredIslemler);
        }

    }
}