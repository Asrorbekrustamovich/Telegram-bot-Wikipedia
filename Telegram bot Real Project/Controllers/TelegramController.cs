using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

[ApiController]
[Route("api/[controller]")]
public class TelegramController : ControllerBase
{
    private readonly IUpdateHandler _telegramBotService;

    public TelegramController(IUpdateHandler telegramBotService)
    {
        _telegramBotService = telegramBotService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        try
        {
            await _telegramBotService.HandleUpdateAsync(null, update, default);
            return Ok();
        }
        catch (Exception ex)
        {
            // Handle exceptions (log or return an error response)
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
}