using GuideAPI.Application.Interfaces;
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
