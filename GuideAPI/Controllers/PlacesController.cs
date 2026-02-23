using GuideAPI.Application.Interfaces;
using GuideAPI.Domain.DTOs;
using GuideAPI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GuideAPI.Controllers
{
    [ApiController]
    [Route("api/place")]
    public class PlacesController:ControllerBase
    {
        private IPlacesService _service { get; set; }

        public PlacesController(IPlacesService service)
        {
            _service = service;
        }

        [HttpPost("google-maps-search-nearby")]
        public async Task<IActionResult> SearchNearby([FromBody]SearchNearbyRequest request, [FromQuery] bool? openNow = null)
        {
            if(request == null)
            {
                return BadRequest("Bad request");
            }
            var result = await _service.SearchNearbyAsync(request);

            // фільтрація, чи відкрито
            if (openNow == true)
            {
                result.Places = result.Places?
                    .Where(p => p.OpenNow == true)
                    .ToArray() ?? Array.Empty<NearbyPlaceDTO>();
            }
            return Ok(result);
        }

        [HttpGet("db-all")]
        public async Task<IActionResult> GetAllFromDb()
        {
            var result = await _service.GetAllPlacesAsync();
            return Ok(result);
        }

        [HttpGet("google-maps-details/{placeId}")]
        public async Task<IActionResult> GetDetails(string placeId, [FromQuery] string? lang = "en") //The language code for Ukrainian is "uk"
        {
            if (string.IsNullOrEmpty(placeId))
            {
                return BadRequest("placeId empty!");
            }

            var result = await _service.GetPlaceDetailsAsync(placeId, lang);
            if(result == null)
            {
                return NotFound("we have no info");
            }
            return Ok(result);
        }

        [HttpPost("google-maps-photo")]
        public async Task<IActionResult> GetPhotoUrls([FromBody]PlacePhotoUrlsRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.PlaceId))
            {
                return BadRequest("PlaceId is required!");
            }

            if ((request.MaxHeightPx.HasValue && (request.MaxHeightPx < 1 || request.MaxHeightPx > 4800)) || (request.MaxWidthPx.HasValue && (request.MaxWidthPx < 1 || request.MaxWidthPx > 4800)))
            {
                return BadRequest("maxHeightPx and maxWidthPx must be between 1 and 4800");
            }

            var photoUrls = await _service.GetPlacePhotoUrlsAsync(request);
            return Ok(photoUrls);
        }



    }
}


