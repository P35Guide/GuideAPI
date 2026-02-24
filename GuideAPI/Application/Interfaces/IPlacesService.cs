using GuideAPI.Domain.DTOs;
using GuideAPI.Domain.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GuideAPI.Application.Interfaces
{
    public interface IPlacesService
    {
        // Search for nearby places by request parameters
        Task<NearbyPlacesResponseDTO> SearchNearbyAsync(SearchNearbyRequest request);

        // Get coordinates from name
        Task<Center?> GetCityCoordinatesByQueryAsync(string query);

        // Convert domain SearchNearbyResponse to DTO
        NearbyPlacesResponseDTO MapToNearbyPlacesResponseDTO(SearchNearbyResponse response);

        // Get detailed information about a place by Id
        Task<NearbyPlaceDTO?> GetPlaceDetailsAsync(string placeId, string languageCode = "en");

        // Get photo URLs for a specific place
        Task<IReadOnlyList<string>> GetPlacePhotoUrlsAsync(PlacePhotoUrlsRequest request);
    }
}
