using GuideAPI.Application.Interfaces;
using GuideAPI.DAL.Abstracts;
using GuideAPI.DAL.Entities;
using GuideAPI.Domain.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace GuideAPI.Application.Services
{
    public class CustomPlacesService : ICustomPlacesService
    {
        private IUserPlaceRepository _repository { get; set; }

        public CustomPlacesService(IUserPlaceRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> AddPlace(CustomPlaceDTO place)
        {
            bool addable = await ChackInfo(place);
            if (addable == false)
            {
                return false;
            }
            await _repository.AddPlaceAsync(MapCustomPlaceToUserPlace(place));
            await _repository.SaveChangesAsync();
            return true;
        }
        public async Task<List<CustomPlaceDTO>> GetAllPlaces()
        {
            List<CustomPlaceDTO> places = new List<CustomPlaceDTO>();
            foreach (var i in await _repository.GetAllPlacesAsync())
            {
                places.Add(MapUserPlaceToCustomPlace(i));
            }
            return places;
        }

        public async Task<CustomPlaceDTO> GetPlaceById(int Id)
        {
            var places = await _repository.GetAllPlacesAsync();
            return MapUserPlaceToCustomPlace(places.FirstOrDefault(p => p.Id == Id));
        }


        private async Task<bool> ChackInfo(CustomPlaceDTO place)
        {
            if (place.NameOfPlace.IsNullOrEmpty())
            {
                return false;
            }
            if (place.Address.IsNullOrEmpty())
            {
                return false;
            }
            if (place.Description.IsNullOrEmpty())
            {
                return false;
            }
            if (place.PhotoUrl.IsNullOrEmpty())
            {
                return false;
            }
            return true;
        }
        private UserPlace MapCustomPlaceToUserPlace(CustomPlaceDTO place)
        {
            return new UserPlace()
            {
                NameOfPlace = place.NameOfPlace,
                Address = place.Address,
                Description = place.Description,
                PhotoUrl = place.PhotoUrl,
            };
        }
        private CustomPlaceDTO MapUserPlaceToCustomPlace(UserPlace place)
        {
            return new CustomPlaceDTO()
            {
                Id = place.Id,
                NameOfPlace = place.NameOfPlace,
                Address = place.Address,
                Description = place.Description,
                PhotoUrl = place.PhotoUrl,
            };
        }
    }
}
