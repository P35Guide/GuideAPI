using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuideAPI.DAL.Entities
{
    public class AppUser
    {
        public int Id { get; set; }

        public long TelegramId { get; set; }

        public ICollection<UserPlace> UserPlaces { get; set; }
    }
}
