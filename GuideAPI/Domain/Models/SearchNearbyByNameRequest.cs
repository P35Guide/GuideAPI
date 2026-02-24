namespace GuideAPI.Domain.Models
{
    public class SearchNearbyByNameRequest
    {
        public string Query { get; set; } = string.Empty; // Назва або ключове слово
        public string[]? IncludedTypes { get; set; } // необов'язкове
        public string[]? ExcludedTypes { get; set; } // необов'язкове
        public int? MaxResultCount { get; set; } // необов'язкове (1-20)
        public string? RankPreference { get; set; } // необов'язкове (POPULARITY або DISTANCE)
        public string? LanguageCode { get; set; } // необов'язкове (uk, en...)
        public LocationRestriction? LocationRestriction { get; set; } // необов'язкове
    }
}