using GuideAPI.DAL.Entities;
using GuideAPI.Domain.DTOs;
using System.Text.Json;

namespace GuideAPI.Application.Services
{
    public static class TelegramUserMapper
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public static TelegramUserDTO? ToDTO(TelegramUser? entity)
        {
            if (entity == null) return null;
            return new TelegramUserDTO
            {
                Id = entity.Id,
                TelegramUserId = entity.TelegramId,
                Settings = entity.Settings != null ? ToSettingsDTO(entity.Settings) : new SettingsDTO { Coordinates = new CoordinatesDTO(), IncludedTypes = new List<string>(), ExcludedTypes = new List<string>() },
                FavoritePlaces = entity.FavoritePlaces?.Select(fp => new FavoritePlaceDTO { Id = fp.PlaceId, Name = fp.Name }).ToList() ?? new List<FavoritePlaceDTO>()
            };
        }

        public static SettingsDTO ToSettingsDTO(UserSettings s)
        {
            return new SettingsDTO
            {
                Id = s.Id,
                Language = s.Language ?? "uk",
                Radius = s.Radius,
                Coordinates = new CoordinatesDTO { Latitude = s.Latitude, Longitude = s.Longitude },
                IncludedTypes = DeserializeList(s.IncludedTypes),
                ExcludedTypes = DeserializeList(s.ExcludedTypes),
                MaxResultCount = s.MaxResultCount,
                RankPreference = s.RankPreference ?? "",
                OpenNow = s.OpenNow
            };
        }

        private static List<string> DeserializeList(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new List<string>();
            try { return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>(); }
            catch { return new List<string>(); }
        }
    }
}
