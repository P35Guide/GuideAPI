using GuideAPI.Domain.DTOs;

namespace GuideAPI.Application.Interfaces
{
    public interface ITelegramUserService
    {
        Task<TelegramUserDTO?> GetByTelegramIdAsync(long telegramId);
        Task<TelegramUserDTO> EnsureUserAsync(long telegramId);
        Task<bool> UpdateSettingsAsync(long telegramId, SettingsDTO settings);
        Task<bool> AddFavoritePlaceAsync(long telegramId, FavoritePlaceDTO place);
        Task<bool> RemoveFavoritePlaceAsync(long telegramId, string placeId);
    }
}
