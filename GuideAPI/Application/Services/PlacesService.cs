using System.Text.Json;
using System.Text;
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

        // -------------------- ПУБЛІЧНІ МЕТОДИ --------------------

        /// <summary>
        /// Виконує пошук місць поблизу за параметрами (координати, типи, радіус).
        /// </summary>
        public async Task<NearbyPlacesResponseDTO> SearchNearbyAsync(SearchNearbyRequest request)
        {
            var radius = request.LocationRestriction.Circle.Radius;
            request.LocationRestriction.Circle.Radius = radius <= 0 ? 1000 : radius;

            var json = await SendNearbySearchApiRequestAsync(request);
            var searchNearbyResponse = DeserializeNearbySearchResponse(json);

            return MapToNearbyPlacesResponseDTO(searchNearbyResponse!);
        }

        /// <summary>
        /// Повертає координати міста або місця за назвою (query).
        /// </summary>
        public async Task<Center?> GetCityCoordinatesByQueryAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query is required.");

            var textSearchJson = await SendTextSearchApiRequestAsync(query);
            return ParseCityCoordinatesFromJson(textSearchJson);
        }

        /// <summary>
        /// Отримує детальну інформацію про місце за його placeId та мовою.
        /// </summary>
        public async Task<NearbyPlaceDTO?> GetPlaceDetailsAsync(string placeId, string languageCode = "en")
        {
            var json = await SendPlaceDetailsApiRequestAsync(placeId, languageCode);
            var place = DeserializePlaceDetailsResponse(json);

            if (place == null)
                return null;

            return MapToNearbyPlaceDTO(place);
        }

        /// <summary>
        /// Конвертує SearchNearbyResponse у DTO для контролера.
        /// </summary>
        public NearbyPlacesResponseDTO MapToNearbyPlacesResponseDTO(SearchNearbyResponse response)
        {
            return new NearbyPlacesResponseDTO
            {
                Places = response.Places?.Select(MapToNearbyPlaceDTO).ToArray() ?? Array.Empty<NearbyPlaceDTO>()
            };
        }

        /// <summary>
        /// Повертає список URL фотографій для конкретного місця.
        /// </summary>
        public async Task<IReadOnlyList<string>> GetPlacePhotoUrlsAsync(PlacePhotoUrlsRequest request)
        {
            var json = await SendPhotoApiRequestAsync(request);
            return ParsePhotoUrlsFromJson(json, request);
        }

        

        // -------------------- ПРИВАТНІ МЕТОДИ --------------------

        /// <summary>
        /// Відправляє запит Nearby Search до Google Places API.
        /// </summary>
        private async Task<string> SendNearbySearchApiRequestAsync(SearchNearbyRequest request)
        {
            var url = $"{BaseUrl}:searchNearby";
            var fieldMask = GetNearbySearchFieldMask("POST");

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Headers.Add("X-Goog-Api-Key", _apiKey);
            httpRequest.Headers.Add("X-Goog-FieldMask", fieldMask);
            httpRequest.Content = JsonContent.Create(request);

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Десеріалізує Nearby Search відповідь у SearchNearbyResponse.
        /// </summary>
        private SearchNearbyResponse? DeserializeNearbySearchResponse(string json)
        {
            return JsonSerializer.Deserialize<SearchNearbyResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        /// <summary>
        /// Відправляє запит Text Search до Google Places API для пошуку міста.
        /// </summary>
        private async Task<string> SendTextSearchApiRequestAsync(string query)
        {
            var textSearchRequest = BuildTextSearchHttpRequest(query);
            var response = await _httpClient.SendAsync(textSearchRequest);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Створює HTTP-запит для Text Search.
        /// </summary>
        private HttpRequestMessage BuildTextSearchHttpRequest(string query)
        {
            var textSearchUrl = $"{BaseUrl}:searchText";
            var textSearchBody = new
            {
                textQuery = query
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, textSearchUrl);
            httpRequest.Headers.Add("X-Goog-Api-Key", _apiKey);
            httpRequest.Headers.Add("X-Goog-FieldMask", "places.location");
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(textSearchBody), Encoding.UTF8, "application/json");
            return httpRequest;
        }


        /// <summary>
        /// Парсить координати міста з JSON-відповіді Text Search.
        /// </summary>
        private Center? ParseCityCoordinatesFromJson(string textSearchJson)
        {
            var placesArray = ExtractPlacesArray(textSearchJson);
            if (placesArray.Length == 0)
                return null;

            return ExtractCoordinatesFromPlace(placesArray[0]);
        }

        /// <summary>
        /// Витягує масив places з JSON-відповіді та повертає клоновані елементи.
        /// </summary>
        private JsonElement[] ExtractPlacesArray(string textSearchJson)
        {
            using var textSearchDoc = JsonDocument.Parse(textSearchJson);
            if (textSearchDoc.RootElement.TryGetProperty("places", out var pe) && pe.ValueKind == JsonValueKind.Array)
            {
                return pe.EnumerateArray().Select(e => e.Clone()).ToArray();
            }
            return Array.Empty<JsonElement>();
        }

        /// <summary>
        /// Витягує координати з елемента місця.
        /// </summary>
        private Center? ExtractCoordinatesFromPlace(JsonElement placeElement)
        {
            if (!placeElement.TryGetProperty("location", out var location))
                return null;

            double latitude = location.GetProperty("latitude").GetDouble();
            double longitude = location.GetProperty("longitude").GetDouble();

            return new Center
            {
                Latitude = latitude,
                Longitude = longitude
            };
        }

        /// <summary>
        /// Відправляє запит на отримання деталей місця.
        /// </summary>
        private async Task<string> SendPlaceDetailsApiRequestAsync(string placeId, string languageCode)
        {
            var httpRequest = BuildPlaceDetailsHttpRequest(placeId, languageCode);
            var response = await _httpClient.SendAsync(httpRequest);
            return await HandlePlaceDetailsApiResponseAsync(response, languageCode);
        }

        /// <summary>
        /// Створює HTTP-запит для отримання деталей місця.
        /// </summary>
        private HttpRequestMessage BuildPlaceDetailsHttpRequest(string placeId, string languageCode)
        {
            var url = $"{BaseUrl}/{placeId}";
            var fieldMask = GetNearbySearchFieldMask("GET");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequest.Headers.Add("X-Goog-Api-Key", _apiKey);
            httpRequest.Headers.Add("X-Goog-FieldMask", fieldMask);

            var uriBuilder = new UriBuilder(url);
            if (!string.IsNullOrWhiteSpace(languageCode))
            {
                uriBuilder.Query = $"languageCode={languageCode}";
            }
            httpRequest.RequestUri = uriBuilder.Uri;

            return httpRequest;
        }

        /// <summary>
        /// Обробляє відповідь на запит деталей місця, перевіряє помилки.
        /// </summary>
        private async Task<string> HandlePlaceDetailsApiResponseAsync(HttpResponseMessage response, string languageCode)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                if (errorJson.Contains("INVALID_ARGUMENT", StringComparison.OrdinalIgnoreCase) &&
                    errorJson.Contains("languageCode", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException($"Invalid language code: {languageCode}");
                }
                return null!;
            }

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Десеріалізує JSON-відповідь деталей місця у Place.
        /// </summary>
        private Place? DeserializePlaceDetailsResponse(string json)
        {
            return JsonSerializer.Deserialize<Place>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        /// <summary>
        /// Відправляє запит до Google Places API для отримання фото місця.
        /// </summary>
        private async Task<string> SendPhotoApiRequestAsync(PlacePhotoUrlsRequest request)
        {
            var url = $"{BaseUrl}/{request.PlaceId}";
            var fieldMask = "photos";
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequest.Headers.Add("X-Goog-Api-Key", _apiKey);
            httpRequest.Headers.Add("X-Goog-FieldMask", fieldMask);

            var response = await _httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode)
                return string.Empty;
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Парсить список URL фотографій з JSON-відповіді.
        /// </summary>
        private IReadOnlyList<string> ParsePhotoUrlsFromJson(string json, PlacePhotoUrlsRequest request)
        {
            if (string.IsNullOrEmpty(json))
                return Array.Empty<string>();

            using var doc = JsonDocument.Parse(json);
            var photoUrls = new List<string>();

            if (doc.RootElement.TryGetProperty("photos", out var photosElement) && photosElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var photo in photosElement.EnumerateArray())
                {
                    var url = BuildPhotoResourceUrl(photo, request);
                    if (!string.IsNullOrEmpty(url))
                        photoUrls.Add(url);
                }
            }
            return photoUrls;
        }

        /// <summary>
        /// Формує URL для отримання фото за photo resource name та параметрами.
        /// </summary>
        private string? BuildPhotoResourceUrl(JsonElement photo, PlacePhotoUrlsRequest request)
        {
            if (!photo.TryGetProperty("name", out var nameElement))
                return null;
            var photoResourceName = nameElement.GetString();
            if (string.IsNullOrEmpty(photoResourceName))
                return null;
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
            return photoResourceUrl;
        }

        /// <summary>
        /// Конвертує Place у NearbyPlaceDTO.
        /// </summary>
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

        /// <summary>
        /// Повертає field mask для Nearby Search/Details запитів.
        /// </summary>
        private static string GetNearbySearchFieldMask(string method)
        {
            if (method == "GET")
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
    }
}
