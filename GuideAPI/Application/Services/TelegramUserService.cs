using GuideAPI.DAL.Abstracts;
using GuideAPI.DAL.Entities;
using GuideAPI.Application.Interfaces;
using GuideAPI.Domain.DTOs;

namespace GuideAPI.Application.Services
{
    public class TelegramUserService : ITelegramUserService
    {
        private readonly ITelegramUserRepository _repository;

        public TelegramUserService(ITelegramUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<TelegramUserDTO?> GetByTelegramIdAsync(long telegramId)
        {
            var user = await _repository.GetByTelegramIdWithDetailsAsync(telegramId);
            return TelegramUserMapper.ToDTO(user);
        }

        public async Task<TelegramUserDTO> EnsureUserAsync(long telegramId)
        {
            var user = await _repository.EnsureUserAsync(telegramId);
            await _repository.SaveChangesAsync();
            var withDetails = await _repository.GetByTelegramIdWithDetailsAsync(telegramId);
            return TelegramUserMapper.ToDTO(withDetails)!;
        }

        public async Task<bool> UpdateSettingsAsync(long telegramId, SettingsDTO settings)
        {
            var user = await _repository.GetByTelegramIdWithDetailsAsync(telegramId);
            if (user == null) return false;

            user.Settings = TelegramUserMapper.ToUserSettingsEntity(settings, user.Id, user.Settings?.Id);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddFavoritePlaceAsync(long telegramId, FavoritePlaceDTO place)
        {
            var user = await _repository.GetByTelegramIdWithDetailsAsync(telegramId);
            if (user == null) return false;
            if (string.IsNullOrWhiteSpace(place.Id) || string.IsNullOrWhiteSpace(place.Name)) return false;

            if (user.FavoritePlaces.Any(fp => fp.PlaceId == place.Id)) return true;

            user.FavoritePlaces.Add(new FavoritePlace
            {
                TelegramUserId = user.Id,
                PlaceId = place.Id,
                Name = place.Name
            });
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFavoritePlaceAsync(long telegramId, string placeId)
        {
            var user = await _repository.GetByTelegramIdWithDetailsAsync(telegramId);
            if (user == null) return false;

            var toRemove = user.FavoritePlaces.FirstOrDefault(fp => fp.PlaceId == placeId);
            if (toRemove == null) return true;

            user.FavoritePlaces.Remove(toRemove);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
