namespace GuideAPI.Domain.DTOs
{
    public class NearbyPlacesResponseDTO
    {
        public NearbyPlaceDTO[] Places { get; set; }
    }

    public class NearbyPlaceDTO
    {
        // Основна інформація
        public string Id { get; set; }
        public string Name { get; set; }
        public string? DisplayName { get; set; }
        public string? PrimaryType { get; set; }

        // Локація
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Рейтинг
        public double? Rating { get; set; }
        public int? UserRatingCount { get; set; }

        // Адреса
        public string? ShortFormattedAddress { get; set; }

        // Контакти
        public string? PhoneNumber { get; set; }
        public string? WebsiteUri { get; set; }
        public string? GoogleMapsUri { get; set; }

        // Ціна
        public string? PriceLevel { get; set; }

        // Години роботи
        public bool? OpenNow { get; set; }
        public string[]? WeekdayDescriptions { get; set; }

        // Опис
        public string? EditorialSummary { get; set; }
        public string? GenerativeSummary { get; set; }
    }
}
