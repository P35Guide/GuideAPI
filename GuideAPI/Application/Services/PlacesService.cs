using System.Text.Json;
using GuideAPI.Application.Interfaces;
using GuideAPI.Domain.DTOs;
using GuideAPI.Domain.Models;

namespace GuideAPI.Application.Services
{
    public class PlacesService : IPlacesService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://places.googleapis.com/v1/places";

        public PlacesService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        // Search for nearby places by request parameters
        public async Task<NearbyPlacesResponseDTO> SearchNearbyAsync(SearchNearbyRequest request)
        {
            var url = $"{BaseUrl}:searchNearby";
            var fieldMask = GetNearbySearchFieldMask();

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Headers.Add("X-Goog-Api-Key", _apiKey);
            httpRequest.Headers.Add("X-Goog-FieldMask", fieldMask);
            httpRequest.Content = JsonContent.Create(request);

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var searchNearbyResponse = JsonSerializer.Deserialize<SearchNearbyResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return MapToNearbyPlacesResponseDTO(searchNearbyResponse!);
        }

        // Get detailed information about a place by Id
        public async Task<NearbyPlaceDTO?> GetPlaceDetailsAsync(string placeId)
        {
            var url = $"{BaseUrl}/{placeId}";
            var fieldMask = GetNearbySearchFieldMask();

            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequest.Headers.Add("X-Goog-Api-Key", _apiKey);
            httpRequest.Headers.Add("X-Goog-FieldMask", fieldMask);

            var response = await _httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var place = JsonSerializer.Deserialize<Place>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (place == null)
                return null;

            return MapToNearbyPlaceDTO(place);
        }

        // Convert domain SearchNearbyResponse to DTO
        public NearbyPlacesResponseDTO MapToNearbyPlacesResponseDTO(SearchNearbyResponse response)
        {
            return new NearbyPlacesResponseDTO
            {
                Places = response.Places?.Select(MapToNearbyPlaceDTO).ToArray() ?? Array.Empty<NearbyPlaceDTO>()
            };
        }

        // Helper: Map Place to NearbyPlaceDTO
        private static NearbyPlaceDTO MapToNearbyPlaceDTO(Place place)
        {
            return new NearbyPlaceDTO
            {
                Id = place.Id,
                Name = place.Name,
                DisplayName = place.DisplayName?.Text,
                PrimaryType = place.PrimaryType,
                Latitude = place.Location?.Latitude ?? 0,
                Longitude = place.Location?.Longitude ?? 0,
                Rating = place.Rating,
                UserRatingCount = place.UserRatingCount,
                ShortFormattedAddress = place.ShortFormattedAddress,
                PhoneNumber = place.NationalPhoneNumber,
                WebsiteUri = place.WebsiteUri,
                GoogleMapsUri = place.GoogleMapsUri,
                PriceLevel = place.PriceLevel,
                OpenNow = place.CurrentOpeningHours?.OpenNow,
                WeekdayDescriptions = place.CurrentOpeningHours?.WeekdayDescriptions,
                EditorialSummary = place.EditorialSummary?.Text,
                GenerativeSummary = place.GenerativeSummary?.Overview?.Text
            };
        }
        // Get photo URLs for a specific place
        public async Task<IReadOnlyList<string>> GetPlacePhotoUrlsAsync(string placeId)
        {
            var url = $"{BaseUrl}/{placeId}";
            var fieldMask = "photos"; // Only request photos field

            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequest.Headers.Add("X-Goog-Api-Key", _apiKey);
            httpRequest.Headers.Add("X-Goog-FieldMask", fieldMask);

            var response = await _httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode)
                return Array.Empty<string>();

            var json = await response.Content.ReadAsStringAsync();

            // Google Places API returns photos as an array of objects with at least a 'name' property (photo resource name)
            // To get the actual photo, you need to request it via: https://places.googleapis.com/v1/{photo.name}
            // For simplicity, return the photo resource URLs

            using var doc = JsonDocument.Parse(json);
            var photoUrls = new List<string>();

            if (doc.RootElement.TryGetProperty("photos", out var photosElement) && photosElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var photo in photosElement.EnumerateArray())
                {
                    if (photo.TryGetProperty("name", out var nameElement))
                    {
                        // Construct the photo resource URL
                        var photoResourceUrl = $"https://places.googleapis.com/v1/{nameElement.GetString()}";
                        photoUrls.Add(photoResourceUrl);
                    }
                }
            }

            return photoUrls;
        }

        // Returns the field mask for Nearby Search requests
        private static string GetNearbySearchFieldMask()
        {
            return string.Join(",",
                "places.id",
                "places.name",
                "places.displayName",
                "places.primaryType",
                "places.location",
                "places.rating",
                "places.userRatingCount",
                "places.shortFormattedAddress",
                "places.nationalPhoneNumber",
                "places.websiteUri",
                "places.googleMapsUri",
                "places.priceLevel",
                "places.currentOpeningHours",
                "places.editorialSummary",
                "places.generativeSummary"
            );
        }
    }
}
