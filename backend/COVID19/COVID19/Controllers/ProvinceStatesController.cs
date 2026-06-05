using COVID19.Data;
using COVID19.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace COVID19.Controllers
{
    public class ProvinceStatesController : ODataController
    {
        private readonly Covid19DbContext _context;

        public ProvinceStatesController(Covid19DbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IQueryable<ProvinceState> Get() => _context.ProvinceStates.AsQueryable();

        [EnableQuery]
        public async Task<ActionResult<ProvinceState>> Get([FromRoute] int key)
        {
            var entity = await _context.ProvinceStates.FirstOrDefaultAsync(x => x.ProvinceStateId == key);
            return entity == null ? NotFound() : Ok(entity);
        }

        public async Task<IActionResult> Post([FromBody] ProvinceState entity)
        {
            _context.ProvinceStates.Add(entity);
            await _context.SaveChangesAsync();
            return Created(entity);
        }

        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] ProvinceState entity)
        {
            var existing = await _context.ProvinceStates.FindAsync(key);
            if (existing == null) return NotFound();

            existing.Name = entity.Name;
            existing.CountryRegionId = entity.CountryRegionId;
            existing.Lat = entity.Lat;
            existing.Long = entity.Long;
            await _context.SaveChangesAsync();
            return Updated(existing);
        }

        public async Task<IActionResult> Patch([FromRoute] int key, [FromBody] Delta<ProvinceState> delta)
        {
            var existing = await _context.ProvinceStates.FindAsync(key);
            if (existing == null) return NotFound();

            delta.Patch(existing);
            await _context.SaveChangesAsync();
            return Updated(existing);
        }

        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var existing = await _context.ProvinceStates.FindAsync(key);
            if (existing == null) return NotFound();

            _context.ProvinceStates.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
