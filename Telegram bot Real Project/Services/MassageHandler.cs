using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram_bot_Real_Project.Interfaces;

public class MessageHandler : IUpdateHandler
{
    private readonly IWikipediaService _wikipediaService;
    private Dictionary<long, string> _userLanguages;

    public MessageHandler(IWikipediaService wikipediaService)
    {
        _wikipediaService = wikipediaService;
        _userLanguages = new Dictionary<long, string>();
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
            int count = 0;
            if (update.Message.Text == "/start" || update.Message.Text == "English" || update.Message.Text == "Russian" || update.Message.Text == "Uzbek")
            {
                _userLanguages.Remove(userId);
            }
            if (_userLanguages.TryGetValue(userId, out string language))
            {
                string wikipediaSummary = await _wikipediaService.GetWikipediaSummaryAsync(userQuery, language);
                botClient.SendTextMessageAsync(chatId: userId, text: wikipediaSummary);
            }


            else
            {
                string lowerUserQuery = userQuery.ToLower();
                if (lowerUserQuery == "uzbek" || lowerUserQuery == "english" || lowerUserQuery == "russian")
                {
                    _userLanguages[userId] = lowerUserQuery == "uzbek" ? "uz" : (lowerUserQuery == "russian" ? "ru" : "en");
                    string languageMessage = $"You have selected '{lowerUserQuery}'. Please enter your query.";
                    botClient.SendTextMessageAsync(chatId: userId, text: languageMessage);
                }
                else if (lowerUserQuery == "/start")
                {
                    string forStartingMessage = "Hello, please select a language: 'Uzbek', 'English', or 'Russian'.";
                    var replyMarkup = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton("Uzbek"),
                        new KeyboardButton("English"),
                        new KeyboardButton("Russian")
                    });
                    botClient.SendTextMessageAsync(chatId: userId, text: forStartingMessage, replyMarkup: replyMarkup);

                }

            }

        }
    }
}

