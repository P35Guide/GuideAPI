using GuideAPI.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuideAPI.DAL.Abstracts
{
    public interface IUserPlaceRepository
    {
        Task<AppUser?> GetUserByTelegramIdAsync(long telegramId);

        Task AddPlaceAsync(UserPlace place);

        
        Task<List<UserPlace>> GetAllPlacesAsync();

        Task SaveChangesAsync();
    }
}