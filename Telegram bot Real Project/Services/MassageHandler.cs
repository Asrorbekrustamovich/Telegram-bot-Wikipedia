using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram_bot_Real_Project.Interfaces;
using Newtonsoft.Json;

public class MessageHandler : IUpdateHandler
{
    
    private readonly IWikipediaService _wikipediaService;

    public MessageHandler()
    {
    }

    public MessageHandler( IWikipediaService wikipediaService)
    {
        _wikipediaService = wikipediaService;
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
       
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message.Type == MessageType.Text)
        {
            var userId = update.Message.Chat.Id;
            string userQuery = update.Message.Text;

            Console.WriteLine($"Received message from user {userId}: {userQuery}");

            string wikipediaSummary =await _wikipediaService.GetWikipediaSummaryAsync(userQuery);
            await botClient.SendTextMessageAsync(userId, wikipediaSummary);
        }
    }
}
