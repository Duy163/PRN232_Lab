namespace COVID19.Entities
{
    public class CovidGlobalDeaths
    {
        public int CovidGlobalDeathsId { get; set; }
        public int? CountryRegionId { get; set; }
        public CountryRegion? CountryRegion { get; set; }
        public int? ProvinceStateId { get; set; }
        public ProvinceState? ProvinceState { get; set; }
        public DateTime ReportDate { get; set; }
        public int Deaths { get; set; }
    }
}
