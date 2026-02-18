namespace GuideAPI.Domain.Models
{
    public class SearchNearbyResponse
    {
        public Place[] Places { get; set; }
    }

    public class Place
    {
        // Обов'язкові поля
        public string Id { get; set; }
        public string Name { get; set; }
        public Location Location { get; set; }

        // Основна інформація
        public DisplayName? DisplayName { get; set; }
        public string? PrimaryType { get; set; }

        // Рейтинг
        public double? Rating { get; set; }
        public int? UserRatingCount { get; set; }

        // Адреса та контакти
        public string? ShortFormattedAddress { get; set; }
        public string? NationalPhoneNumber { get; set; }
        public string? WebsiteUri { get; set; }
        public string? GoogleMapsUri { get; set; }

        // Ціна
        public string? PriceLevel { get; set; }

        // Години роботи
        public OpeningHours? CurrentOpeningHours { get; set; }

        // Описи
        public TextObject? EditorialSummary { get; set; }
        public GenerativeSummary? GenerativeSummary { get; set; }
    }

    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class DisplayName
    {
        public string? Text { get; set; }
        public string? LanguageCode { get; set; }
    }

    public class OpeningHours
    {
        public bool? OpenNow { get; set; }
        public string[]? WeekdayDescriptions { get; set; }
    }

    public class TextObject
    {
        public string? Text { get; set; }
        public string? LanguageCode { get; set; }
    }

    public class GenerativeSummary
    {
        public TextObject? Overview { get; set; }
    }
}
