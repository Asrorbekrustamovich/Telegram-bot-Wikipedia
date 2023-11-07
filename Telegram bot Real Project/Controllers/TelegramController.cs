using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

[ApiController]
[Route("api/[controller]")]
public class TelegramController : ControllerBase
{
    private readonly ITelegramBotClient _botClient;

    public TelegramController(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        if (update == null)
        {
            return BadRequest();
        }

        if (update.Message != null && update.Message.Type == MessageType.Text)
        {
            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            // Create a button
            var button = new KeyboardButton("Click me!");

            // Create a keyboard with the button
            var keyboard = new ReplyKeyboardMarkup(new[] { new[] { button } });

            // Process the incoming message and send a message with the button
            await _botClient.SendTextMessageAsync(chatId, $"You said: {messageText}", replyMarkup: keyboard);

            return Ok();
        }

        return NotFound();
    }
}
