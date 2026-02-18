using GuideAPI.Application.Interfaces;
using GuideAPI.Domain.DTOs;
using GuideAPI.Domain.Models;

namespace GuideAPI.Application.Services
{
    public class PlacesService : IPlacesService
    {
        public string GenerateGoogleMapsLink(string placeId)
        {
            throw new NotImplementedException();
        }

        public Task<NearbyPlaceDTO?> GetPlaceDetailsAsync(string placeId)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<string>> GetPlacePhotoUrlsAsync(string placeId)
        {
            throw new NotImplementedException();
        }

        public NearbyPlacesResponseDTO MapToNearbyPlacesResponseDTO(SearchNearbyResponse response)
        {
            throw new NotImplementedException();
        }

        public Task<NearbyPlacesResponseDTO> SearchNearbyAsync(SearchNearbyRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
