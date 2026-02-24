using GuideAPI.Application.Interfaces;
using GuideAPI.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GuideAPI.Controllers
{
    [ApiController]
    [Route("api/custom")]
    public class CustomPlaceController:ControllerBase
    {
        ICustomPlacesService _service { get; set; }
        public CustomPlaceController(ICustomPlacesService service)
        {
            _service = service;
        }
        [HttpPost("addPlace")]
        public async Task<IActionResult> AddPlace([FromBody]CustomPlaceDTO placeDTO)
        {
            var result = await _service.AddPlace(placeDTO);
            if(result == true)
            {
                return Ok("place added");
            }
            return BadRequest("info uncorrect");
        }

        [HttpGet("getAllPlaces")]
        public async Task<IActionResult> GetAllPlaces()
        {
            var result = await _service.GetAllPlaces();
            if(result != null)
            {
                return Ok(result);
            }
            return BadRequest("we have no info");
        }

        [HttpGet("getPlaceById")]
        public async Task<IActionResult> GetPlaceById(int Id)
        {
            var result = await _service.GetPlaceById(Id);
            if(result == null)
            {
                return NotFound("uncorrect id");
            }
            return Ok(result);
        }
    }
}
