namespace GuideAPI.Domain.DTOs
{
    public class TelegramUserDTO
    {
        public int Id { get; set; }
        public long TelegramUserId { get; set; }  
        public SettingsDTO Settings { get; set; }
        public List<FavoritePlaceDTO> FavoritePlaces { get; set; }
    }

    public class FavoritePlaceDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
