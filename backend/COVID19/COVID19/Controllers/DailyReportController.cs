using COVID19.Data;
using COVID19.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace COVID19.Controllers
{
    public class DailyReportController : ODataController
    {
        private readonly Covid19DbContext _context;

        public DailyReportController(Covid19DbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IQueryable<DailyReport> Get() => _context.DailyReports.AsQueryable();

        [EnableQuery]
        public async Task<ActionResult<DailyReport>> Get([FromRoute] int key)
        {
            var entity = await _context.DailyReports
                .Include(x => x.CountryRegion)
                .Include(x => x.ProvinceState)
                .FirstOrDefaultAsync(x => x.DailyReportId == key);
            return entity == null ? NotFound() : Ok(entity);
        }

        [EnableQuery]
        public async Task<ActionResult<IQueryable<DailyReport>>> ByCountryRegion([FromRoute] int key)
        {
            var query = _context.DailyReports
                .Include(x => x.CountryRegion)
                .Include(x => x.ProvinceState)
                .Where(x => x.CountryRegionId == key)
                .AsQueryable();

            return Ok(query);
        }

        public async Task<IActionResult> Post([FromBody] DailyReport entity)
        {
            _context.DailyReports.Add(entity);
            await _context.SaveChangesAsync();
            return Created(entity);
        }

        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] DailyReport entity)
        {
            var existing = await _context.DailyReports.FindAsync(key);
            if (existing == null) return NotFound();

            existing.CountryRegionId = entity.CountryRegionId;
            existing.ProvinceStateId = entity.ProvinceStateId;
            existing.ReportDate = entity.ReportDate;
            existing.UID = entity.UID;
            existing.ISO3 = entity.ISO3;
            existing.FIPS = entity.FIPS;
            existing.Confirmed = entity.Confirmed;
            existing.Deaths = entity.Deaths;
            existing.Recovered = entity.Recovered;
            existing.Active = entity.Active;
            existing.IncidentRate = entity.IncidentRate;
            existing.PeopleTested = entity.PeopleTested;
            existing.PeopleHospitalized = entity.PeopleHospitalized;
            existing.TestingRate = entity.TestingRate;
            existing.HospitalizationRate = entity.HospitalizationRate;
            await _context.SaveChangesAsync();
            return Updated(existing);
        }

        public async Task<IActionResult> Patch([FromRoute] int key, [FromBody] Delta<DailyReport> delta)
        {
            var existing = await _context.DailyReports.FindAsync(key);
            if (existing == null) return NotFound();

            delta.Patch(existing);
            await _context.SaveChangesAsync();
            return Updated(existing);
        }

        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var existing = await _context.DailyReports.FindAsync(key);
            if (existing == null) return NotFound();

            _context.DailyReports.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
