using COVID19.Entities;
using Microsoft.EntityFrameworkCore;

namespace COVID19.Data
{
    public class Covid19DbContext : DbContext
    {
        public Covid19DbContext(DbContextOptions<Covid19DbContext> options) : base(options)
        {
        }

        public DbSet<CountryRegion> CountryRegions => Set<CountryRegion>();
        public DbSet<ProvinceState> ProvinceStates => Set<ProvinceState>();
        public DbSet<CovidGlobalConfirmed> CovidGlobalConfirmeds => Set<CovidGlobalConfirmed>();
        public DbSet<CovidGlobalDeaths> CovidGlobalDeaths => Set<CovidGlobalDeaths>();
        public DbSet<CovidGlobalRecovered> CovidGlobalRecovereds => Set<CovidGlobalRecovered>();
        public DbSet<DailyReport> DailyReports => Set<DailyReport>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CountryRegion>()
                .HasMany(c => c.ProvinceStates)
                .WithOne(p => p.CountryRegion)
                .HasForeignKey(p => p.CountryRegionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
