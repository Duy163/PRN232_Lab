using COVID19.Data;
using COVID19.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace COVID19.Controllers
{
    public class CovidGlobalRecoveredsController : ODataController
    {
        private readonly Covid19DbContext _context;

        public CovidGlobalRecoveredsController(Covid19DbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IQueryable<CovidGlobalRecovered> Get() => _context.CovidGlobalRecovereds.AsQueryable();

        [EnableQuery]
        public async Task<ActionResult<CovidGlobalRecovered>> Get([FromRoute] int key)
        {
            var entity = await _context.CovidGlobalRecovereds.FirstOrDefaultAsync(x => x.CovidGlobalRecoveredId == key);
            return entity == null ? NotFound() : Ok(entity);
        }

        public async Task<IActionResult> Post([FromBody] CovidGlobalRecovered entity)
        {
            _context.CovidGlobalRecovereds.Add(entity);
            await _context.SaveChangesAsync();
            return Created(entity);
        }

        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] CovidGlobalRecovered entity)
        {
            var existing = await _context.CovidGlobalRecovereds.FindAsync(key);
            if (existing == null) return NotFound();

            existing.CountryRegionId = entity.CountryRegionId;
            existing.ProvinceStateId = entity.ProvinceStateId;
            existing.ReportDate = entity.ReportDate;
            existing.RecoveredCases = entity.RecoveredCases;
            await _context.SaveChangesAsync();
            return Updated(existing);
        }

        public async Task<IActionResult> Patch([FromRoute] int key, [FromBody] Delta<CovidGlobalRecovered> delta)
        {
            var existing = await _context.CovidGlobalRecovereds.FindAsync(key);
            if (existing == null) return NotFound();

            delta.Patch(existing);
            await _context.SaveChangesAsync();
            return Updated(existing);
        }

        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var existing = await _context.CovidGlobalRecovereds.FindAsync(key);
            if (existing == null) return NotFound();

            _context.CovidGlobalRecovereds.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
