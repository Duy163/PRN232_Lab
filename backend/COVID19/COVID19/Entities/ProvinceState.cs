namespace COVID19.Entities
{
    public class ProvinceState
    {
        public int ProvinceStateId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? CountryRegionId { get; set; }
        public CountryRegion? CountryRegion { get; set; }
        public ICollection<CovidGlobalConfirmed> CovidGlobalConfirmeds { get; set; } = new List<CovidGlobalConfirmed>();
        public ICollection<CovidGlobalDeaths> CovidGlobalDeaths { get; set; } = new List<CovidGlobalDeaths>();
        public ICollection<CovidGlobalRecovered> CovidGlobalRecovereds { get; set; } = new List<CovidGlobalRecovered>();
        public ICollection<DailyReport> DailyReports { get; set; } = new List<DailyReport>();
        public double? Lat { get; set; }
        public double? Long { get; set; }
    }
}
