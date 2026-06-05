using System.ComponentModel.DataAnnotations.Schema;

namespace COVID19.Entities
{
    [Table("DailyReport")]
    public class DailyReport
    {
        public int DailyReportId { get; set; }
        public int? CountryRegionId { get; set; }
        public CountryRegion? CountryRegion { get; set; }
        public int? ProvinceStateId { get; set; }
        public ProvinceState? ProvinceState { get; set; }
        public DateTime ReportDate { get; set; }
        public int? UID { get; set; }
        public string? ISO3 { get; set; }
        public int? FIPS { get; set; }
        public int Confirmed { get; set; }
        public int Deaths { get; set; }
        public int Recovered { get; set; }
        public int Active { get; set; }
        public double? IncidentRate { get; set; }
        public long? PeopleTested { get; set; }
        public long? PeopleHospitalized { get; set; }
        public double? TestingRate { get; set; }
        public double? HospitalizationRate { get; set; }
    }
}
