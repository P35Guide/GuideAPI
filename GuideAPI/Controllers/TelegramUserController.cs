using GuideAPI.Application.Interfaces;
using GuideAPI.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GuideAPI.Controllers
{
    [ApiController]
    [Route("api/telegram-user")]
    public class TelegramUserController : ControllerBase
    {
        private readonly ITelegramUserService _service;

        public TelegramUserController(ITelegramUserService service)
        {
            _service = service;
        }

        [HttpGet("{telegramId:long}")]
        public async Task<IActionResult> GetByTelegramId(long telegramId)
        {
            var result = await _service.GetByTelegramIdAsync(telegramId);
            if (result == null)
                return NotFound("User not found.");
            return Ok(result);
        }

        [HttpPost("{telegramId:long}")]
        public async Task<IActionResult> EnsureUser(long telegramId)
        {
            var result = await _service.EnsureUserAsync(telegramId);
            return Ok(result);
        }

        [HttpPost("{telegramId:long}/settings")]
        public async Task<IActionResult> UpdateSettings(long telegramId, [FromBody] SettingsDTO settings)
        {
            if (settings == null)
                return BadRequest("Settings are required.");
            var success = await _service.UpdateSettingsAsync(telegramId, settings);
            if (!success)
                return NotFound("User not found.");
            return Ok();
        }

        [HttpPost("{telegramId:long}/favorites")]
        public async Task<IActionResult> AddFavoritePlace(long telegramId, [FromBody] FavoritePlaceDTO place)
        {
            if (place == null)
                return BadRequest("Place is required.");
            var success = await _service.AddFavoritePlaceAsync(telegramId, place);
            if (!success)
                return NotFound("User not found.");
            return Ok();
        }

        [HttpPost("{telegramId:long}/favorites/remove")]
        public async Task<IActionResult> RemoveFavoritePlace(long telegramId, [FromBody] RemoveFavoriteRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PlaceId))
                return BadRequest("placeId is required.");
            var success = await _service.RemoveFavoritePlaceAsync(telegramId, request.PlaceId);
            if (!success)
                return NotFound("User not found.");
            return Ok();
        }
    }

    public class RemoveFavoriteRequest
    {
        public string PlaceId { get; set; } = string.Empty;
    }
}
