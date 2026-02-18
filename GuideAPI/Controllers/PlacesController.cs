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

        [HttpGet("google-maps-search-nearby/{placeId}")]
        public async Task<IActionResult> SearchNearby(double Latitude,double Longitude,double Radius)
        {
            Radius = Radius == 0 ? 1000 : Radius;
            var res = await _service.SearchNearbyAsync(
                new SearchNearbyRequest()
                {
                    LocationRestriction = new LocationRestriction()
                    {
                        Circle = new Circle()
                        {
                            Radius = Radius,
                            Center = new Center()
                            {
                                Latitude = Latitude,
                                Longitude = Longitude
                            }
                        }
                    }
                }
                );
            return Ok(res);
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
