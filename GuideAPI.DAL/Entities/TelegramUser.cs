namespace GuideAPI.DAL.Entities
{
    public class TelegramUser
    {
        public int Id { get; set; }
        public long TelegramId { get; set; }

        public UserSettings? Settings { get; set; }
        public List<FavoritePlace> FavoritePlaces { get; set; } = new();
    }
}
