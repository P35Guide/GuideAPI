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
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
        public byte[] Photo1 { get; set; }
        public byte[] Photo2 { get; set; }
        public byte[] Photo3 { get; set; }
        public byte[] Photo4 { get; set; }
        public byte[] Photo5 { get; set; }

    }
}
