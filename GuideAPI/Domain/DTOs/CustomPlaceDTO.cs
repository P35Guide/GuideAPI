using GuideAPI.DAL.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GuideAPI.Domain.DTOs
{
    public class CustomPlaceDTO
    {
        public int Id { get; set; }
        public string NameOfPlace { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
    }
}
