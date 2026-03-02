using GuideAPI.DAL.Abstracts;
using GuideAPI.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace GuideAPI.DAL
{
    public class TelegramUserRepository : ITelegramUserRepository
    {
        private readonly AppDbContext _context;

        public TelegramUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TelegramUser?> GetByTelegramIdWithDetailsAsync(long telegramId)
        {
            return await _context.TelegramUsers
                .Include(u => u.Settings)
                .Include(u => u.FavoritePlaces)
                .FirstOrDefaultAsync(u => u.TelegramId == telegramId);
        }

        public async Task<TelegramUser> EnsureUserAsync(long telegramId)
        {
            var user = await _context.TelegramUsers.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
            if (user != null) return user;
            user = new TelegramUser { TelegramId = telegramId };
            await _context.TelegramUsers.AddAsync(user);
            return user;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
