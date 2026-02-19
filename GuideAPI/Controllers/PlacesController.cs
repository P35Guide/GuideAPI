using GuideAPI.Application.Interfaces;
using GuideAPI.Domain.Models;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> SearchNearby([FromBody]SearchNearbyRequest request)
        {
            if(request == null)
            {
                return BadRequest("Bad request");
            }
            var result = await _service.SearchNearbyAsync(request);
            return Ok(result);
        }

        [HttpGet("google-maps-details/{placeId}")]
        public async Task<IActionResult> GetDetails(string placeId)
        {
            if (string.IsNullOrEmpty(placeId))
            {
                return BadRequest("placeId empty!");
            }

            var res = await _service.GetPlaceDetailsAsync(placeId);
            return Ok(res);
        }

        [HttpGet("google-maps-photo/{placeId}")]
        public async Task<IActionResult> GetPhotoUrls(string placeId)
        {
            if (string.IsNullOrEmpty(placeId))
            {
                return BadRequest("placeId empty!");
            }

            var photoUrls = await _service.GetPlacePhotoUrlsAsync(placeId);
            return Ok(photoUrls);
        }



    }
}
