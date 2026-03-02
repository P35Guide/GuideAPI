using GuideAPI.Application.Interfaces;
using GuideAPI.DAL.Abstracts;
using GuideAPI.DAL.Entities;
using GuideAPI.Domain.DTOs;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

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
            if (!addable)
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
            var found = places.FirstOrDefault(p => p.Id == Id);
            return MapUserPlaceToCustomPlace(found);
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
            if (place.Photo1 == null)
            {
                return false;
            }
            if (place.Photo2 == null)
            {
                return false;
            }
            if (place.Photo3 == null)
            {
                return false;
            }
            if (place.Photo4 == null)
            {
                return false;
            }
            if (place.Photo5 == null)
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
                Latitude = place.Latitude, // not required, default 0 if not set
                Longitude = place.Longitude, // not required, default 0 if not set
                Photo1 = place.Photo1,
                Photo2 = place.Photo2,
                Photo3 = place.Photo3,
                Photo4 = place.Photo4,
                Photo5 = place.Photo5
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
                Latitude = place.Latitude, // not required, default 0 if not set
                Longitude = place.Longitude, // not required, default 0 if not set
                Photo1 = place.Photo1,
                Photo2 = place.Photo2,
                Photo3 = place.Photo3,
                Photo4 = place.Photo4,
                Photo5 = place.Photo5
            };
        }
    }
}
