using System.ComponentModel.DataAnnotations;

namespace GuideAPI.DAL.Entities
{
    public class UserSettings
    {
        public int Id { get; set; }
        public int TelegramUserId { get; set; }
        public TelegramUser TelegramUser { get; set; } = null!;

        [MaxLength(10)]
        public string Language { get; set; } = "uk";
        public int Radius { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        /// <summary>JSON array of strings</summary>
        public string? IncludedTypes { get; set; }
        /// <summary>JSON array of strings</summary>
        public string? ExcludedTypes { get; set; }
        public int MaxResultCount { get; set; }
        [MaxLength(50)]
        public string? RankPreference { get; set; }
        public bool OpenNow { get; set; }
    }
}
