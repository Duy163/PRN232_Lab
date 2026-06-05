using COVID19.Data;
using COVID19.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace COVID19.Controllers
{
    public class CountryRegionsController : ODataController
    {
        private readonly Covid19DbContext _context;

        public CountryRegionsController(Covid19DbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IQueryable<CountryRegion> Get() => _context.CountryRegions.AsQueryable();

        [EnableQuery]
        public async Task<ActionResult<CountryRegion>> Get([FromRoute] int key)
        {
            var entity = await _context.CountryRegions.FirstOrDefaultAsync(x => x.CountryRegionId == key);
            return entity == null ? NotFound() : Ok(entity);
        }

        public async Task<IActionResult> Post([FromBody] CountryRegion entity)
        {
            _context.CountryRegions.Add(entity);
            await _context.SaveChangesAsync();
            return Created(entity);
        }

        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] CountryRegion entity)
        {
            var existing = await _context.CountryRegions.FindAsync(key);
            if (existing == null) return NotFound();

            existing.Name = entity.Name;
            await _context.SaveChangesAsync();
            return Updated(existing);
        }

        public async Task<IActionResult> Patch([FromRoute] int key, [FromBody] Delta<CountryRegion> delta)
        {
            var existing = await _context.CountryRegions.FindAsync(key);
            if (existing == null) return NotFound();

            delta.Patch(existing);
            await _context.SaveChangesAsync();
            return Updated(existing);
        }

        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var existing = await _context.CountryRegions.FindAsync(key);
            if (existing == null) return NotFound();

            _context.CountryRegions.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
