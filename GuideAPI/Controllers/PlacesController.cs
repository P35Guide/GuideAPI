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



    }
}
