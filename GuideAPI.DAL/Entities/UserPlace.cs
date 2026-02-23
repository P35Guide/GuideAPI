using System.ComponentModel.DataAnnotations;

namespace GuideAPI.DAL.Entities
{
    public class UserPlace
    {
        public int Id { get; set; }

        [Required]
        public string NameOfPlace { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string PhotoUrl { get; set; }

       
    }
}