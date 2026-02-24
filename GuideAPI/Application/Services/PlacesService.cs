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
            var radius = request.LocationRestriction.Circle.Radius;
            request.LocationRestriction.Circle.Radius = radius <=0?1000:radius;
            var url = $"{BaseUrl}:searchNearby";
            var fieldMask = GetNearbySearchFieldMask("POST");

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

        // Search for nearby places by name and/or coordinates
        public async Task<NearbyPlacesResponseDTO> SearchNearbyByNameAsync(SearchNearbyByNameRequest request)
        {
            // Check for empty query and coordinates
            bool hasQuery = !string.IsNullOrWhiteSpace(request.Query);
            bool hasCoordinates = HasCoordinates(request);

            if (!hasQuery && !hasCoordinates)
                throw new ArgumentException("Search requires either a name (query) or coordinates.");

            // If query is empty, search by coordinates only
            if (!hasQuery && hasCoordinates)
            {
                var nearbyRequest = MapToNearbyRequest(request);
                return await SearchNearbyAsync(nearbyRequest);
            }

            // If coordinates are missing, search by place name (query) only
            if (hasQuery && !hasCoordinates)
            {
                // Implement Text Search API call here for search by name only
                throw new NotImplementedException("Text Search API for name-only search is not implemented.");
            }

            // If both query and coordinates are present, search for places matching the name near the coordinates
            // Implement Text Search API call here for search by name near coordinates
            throw new NotImplementedException("Text Search API for name-near-coordinates search is not implemented.");
        }

        // Get detailed information about a place by Id and optional language
        public async Task<NearbyPlaceDTO?> GetPlaceDetailsAsync(string placeId, string languageCode = "en")
        {
            var url = $"{BaseUrl}/{placeId}";
            var fieldMask = GetNearbySearchFieldMask("GET");

            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequest.Headers.Add("X-Goog-Api-Key", _apiKey);
            httpRequest.Headers.Add("X-Goog-FieldMask", fieldMask);

            // Add languageCode only if provided and not empty
            var uriBuilder = new UriBuilder(url);
            if (!string.IsNullOrWhiteSpace(languageCode))
            {
                uriBuilder.Query = $"languageCode={languageCode}";
            }
            httpRequest.RequestUri = uriBuilder.Uri;

            var response = await _httpClient.SendAsync(httpRequest);

            // Check for invalid language code error
            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                if (errorJson.Contains("INVALID_ARGUMENT", StringComparison.OrdinalIgnoreCase) &&
                    errorJson.Contains("languageCode", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException($"Invalid language code: {languageCode}");
                }
                return null;
            }

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

       
        // Get photo URLs for a specific place
        public async Task<IReadOnlyList<string>> GetPlacePhotoUrlsAsync(PlacePhotoUrlsRequest request)
        {
            var url = $"{BaseUrl}/{request.PlaceId}";
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
                        var photoResourceName = nameElement.GetString();
                        // Construct the photo resource URL
                        var photoResourceUrl = $"https://places.googleapis.com/v1/{photoResourceName}/media?key={_apiKey}";

                        if (request.MaxHeightPx.HasValue || request.MaxWidthPx.HasValue)
                        {
                            if (request.MaxHeightPx.HasValue)
                                photoResourceUrl += $"&maxHeightPx={Math.Clamp(request.MaxHeightPx.Value, 1, 4800)}";
                            if (request.MaxWidthPx.HasValue)
                                photoResourceUrl += $"&maxWidthPx={Math.Clamp(request.MaxWidthPx.Value, 1, 4800)}";
                        }
                        else
                        {
                            photoResourceUrl += "&maxHeightPx=400";
                        }
                            photoUrls.Add(photoResourceUrl);
                    }
                }
            }

            return photoUrls;
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

        // Returns the field mask for Nearby Search requests
        private static string GetNearbySearchFieldMask(string method)
        {
            if(method == "GET")
            {
                return string.Join(",",
                "id",
                "name",
                "displayName",
                "primaryType",
                "location",
                "rating",
                "userRatingCount",
                "shortFormattedAddress",
                "nationalPhoneNumber",
                "websiteUri",
                "googleMapsUri",
                "priceLevel",
                "currentOpeningHours",
                "editorialSummary",
                "generativeSummary"
            );
            }
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

        private bool HasCoordinates(SearchNearbyByNameRequest request)
        {
            return request.LocationRestriction != null &&
                   request.LocationRestriction.Circle != null &&
                   request.LocationRestriction.Circle.Center != null &&
                   request.LocationRestriction.Circle.Center.Latitude != null &&
                   request.LocationRestriction.Circle.Center.Longitude != null &&
                   request.LocationRestriction.Circle.Radius != null;
        }

        private SearchNearbyRequest MapToNearbyRequest(SearchNearbyByNameRequest request)
        {
            return new SearchNearbyRequest
            {
                IncludedTypes = request.IncludedTypes,
                ExcludedTypes = request.ExcludedTypes,
                MaxResultCount = request.MaxResultCount,
                LanguageCode = request.LanguageCode,
                RankPreference = request.RankPreference,
                LocationRestriction = request.LocationRestriction
            };
        }
    }
}
