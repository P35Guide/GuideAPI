using System.ComponentModel.DataAnnotations;

namespace GuideAPI.DAL.Entities
{
    public class FavoritePlace
    {
        public int Id { get; set; }
        public int TelegramUserId { get; set; }
        public TelegramUser TelegramUser { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string PlaceId { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}
