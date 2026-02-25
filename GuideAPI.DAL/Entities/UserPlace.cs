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
        public byte[] Photo1 { get; set; }
        [Required]
        public byte[] Photo2 { get; set; }
        [Required]
        public byte[] Photo3 { get; set; }
        [Required]
        public byte[] Photo4 { get; set; }
        [Required]
        public byte[] Photo5 { get; set; }

    }
}