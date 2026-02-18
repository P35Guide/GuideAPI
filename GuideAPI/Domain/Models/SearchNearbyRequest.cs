namespace GuideAPI.Domain.Models
{
    public class SearchNearbyRequest
    {
        public string[]? IncludedTypes { get; set; } // необов'язкове
        public string[]? ExcludedTypes { get; set; } // необов'язкове
        public int? MaxResultCount { get; set; } // необов'язкове (1-20)
        public string? RankPreference { get; set; } // необов'язкове (POPULARITY або DISTANCE)
        public string? LanguageCode { get; set; } // необов'язкове (uk, en...)
        public LocationRestriction LocationRestriction { get; set; }
    }

    public class LocationRestriction
    {
        public Circle Circle { get; set; }
    }

    public class Circle
    {
        public Center Center { get; set; }
        public double Radius { get; set; }
    }

    public class Center
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
