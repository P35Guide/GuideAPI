using GuideAPI.DAL.Abstracts;
using GuideAPI.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuideAPI.DAL
{
    public class UserPlaceRepository : IUserPlaceRepository
    {
        private readonly AppDbContext _context;

        public UserPlaceRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<AppUser?> GetUserByTelegramIdAsync(long telegramId)
        {
            return await _context.AppUsers
                .FirstOrDefaultAsync(u => u.TelegramId == telegramId);
        }
        public async Task AddPlaceAsync(Entities.UserPlace place)
        {
            await _context.UserPlaces.AddAsync(place);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<UserPlace>> GetAllPlacesAsync()
        {
            return await _context.UserPlaces.ToListAsync();
        }
    }
}
