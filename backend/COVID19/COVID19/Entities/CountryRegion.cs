namespace COVID19.Entities
{
    public class CountryRegion
    {
        public int CountryRegionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<ProvinceState> ProvinceStates { get; set; } = new List<ProvinceState>();
    }
}
