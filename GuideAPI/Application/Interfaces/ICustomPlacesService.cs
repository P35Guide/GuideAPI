using GuideAPI.Domain.DTOs;

namespace GuideAPI.Application.Interfaces
{
    public interface ICustomPlacesService
    {
        public  Task<bool> AddPlace(CustomPlaceDTO place);
        public  Task<List<CustomPlaceDTO>> GetAllPlaces();
        public Task<CustomPlaceDTO> GetPlaceById(int Id);

    }
}
