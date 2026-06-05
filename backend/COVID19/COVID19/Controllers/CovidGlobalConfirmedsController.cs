using COVID19.Data;
using COVID19.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace COVID19.Controllers
{
    public class CovidGlobalConfirmedsController : ODataController
    {
        private readonly Covid19DbContext _context;

        public CovidGlobalConfirmedsController(Covid19DbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IQueryable<CovidGlobalConfirmed> Get() => _context.CovidGlobalConfirmeds.AsQueryable();

        [EnableQuery]
        public async Task<ActionResult<CovidGlobalConfirmed>> Get([FromRoute] int key)
        {
            var entity = await _context.CovidGlobalConfirmeds.FirstOrDefaultAsync(x => x.CovidGlobalConfirmedId == key);
            return entity == null ? NotFound() : Ok(entity);
        }

        public async Task<IActionResult> Post([FromBody] CovidGlobalConfirmed entity)
        {
            _context.CovidGlobalConfirmeds.Add(entity);
            await _context.SaveChangesAsync();
            return Created(entity);
        }

        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] CovidGlobalConfirmed entity)
        {
            var existing = await _context.CovidGlobalConfirmeds.FindAsync(key);
            if (existing == null) return NotFound();

            existing.CountryRegionId = entity.CountryRegionId;
            existing.ProvinceStateId = entity.ProvinceStateId;
            existing.ReportDate = entity.ReportDate;
            existing.ConfirmedCases = entity.ConfirmedCases;
            await _context.SaveChangesAsync();
            return Updated(existing);
        }

        public async Task<IActionResult> Patch([FromRoute] int key, [FromBody] Delta<CovidGlobalConfirmed> delta)
        {
            var existing = await _context.CovidGlobalConfirmeds.FindAsync(key);
            if (existing == null) return NotFound();

            delta.Patch(existing);
            await _context.SaveChangesAsync();
            return Updated(existing);
        }

        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var existing = await _context.CovidGlobalConfirmeds.FindAsync(key);
            if (existing == null) return NotFound();

            _context.CovidGlobalConfirmeds.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
