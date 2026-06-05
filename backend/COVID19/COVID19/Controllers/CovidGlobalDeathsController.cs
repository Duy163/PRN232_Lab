using COVID19.Data;
using COVID19.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace COVID19.Controllers
{
    public class CovidGlobalDeathsController : ODataController
    {
        private readonly Covid19DbContext _context;

        public CovidGlobalDeathsController(Covid19DbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IQueryable<CovidGlobalDeaths> Get() => _context.CovidGlobalDeaths.AsQueryable();

        [EnableQuery]
        public async Task<ActionResult<CovidGlobalDeaths>> Get([FromRoute] int key)
        {
            var entity = await _context.CovidGlobalDeaths.FirstOrDefaultAsync(x => x.CovidGlobalDeathsId == key);
            return entity == null ? NotFound() : Ok(entity);
        }

        public async Task<IActionResult> Post([FromBody] CovidGlobalDeaths entity)
        {
            _context.CovidGlobalDeaths.Add(entity);
            await _context.SaveChangesAsync();
            return Created(entity);
        }

        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] CovidGlobalDeaths entity)
        {
            var existing = await _context.CovidGlobalDeaths.FindAsync(key);
            if (existing == null) return NotFound();

            existing.CountryRegionId = entity.CountryRegionId;
            existing.ProvinceStateId = entity.ProvinceStateId;
            existing.ReportDate = entity.ReportDate;
            existing.Deaths = entity.Deaths;
            await _context.SaveChangesAsync();
            return Updated(existing);
        }

        public async Task<IActionResult> Patch([FromRoute] int key, [FromBody] Delta<CovidGlobalDeaths> delta)
        {
            var existing = await _context.CovidGlobalDeaths.FindAsync(key);
            if (existing == null) return NotFound();

            delta.Patch(existing);
            await _context.SaveChangesAsync();
            return Updated(existing);
        }

        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var existing = await _context.CovidGlobalDeaths.FindAsync(key);
            if (existing == null) return NotFound();

            _context.CovidGlobalDeaths.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
