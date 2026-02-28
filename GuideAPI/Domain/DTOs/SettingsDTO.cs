namespace GuideAPI.Domain.DTOs
{
    public class SettingsDTO
    {
        public int Id { get; set; }
        public string Language { get; set; }
        public int Radius { get; set; }
        public CoordinatesDTO Coordinates { get; set; }
        public List<string> IncludedTypes { get; set; }
        public List<string> ExcludedTypes { get; set; }
        public int MaxResultCount { get; set; }
        public string RankPreference { get; set; }
        public bool OpenNow { get; set; }
    }

    public class CoordinatesDTO
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
