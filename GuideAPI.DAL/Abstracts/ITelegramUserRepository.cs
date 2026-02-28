using GuideAPI.DAL.Entities;

namespace GuideAPI.DAL.Abstracts
{
    public interface ITelegramUserRepository
    {
        Task<TelegramUser?> GetByTelegramIdWithDetailsAsync(long telegramId);
        Task<TelegramUser> EnsureUserAsync(long telegramId);
        Task SaveChangesAsync();
    }
}
