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
using System.Net;
using System.Text.RegularExpressions;

public class MessageHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IWikipediaService _wikipediaService;

    public MessageHandler()
    {
    }

    public MessageHandler(ITelegramBotClient telegramBotClient, WebClient webClient)
    {
        _telegramBotClient = telegramBotClient;
       
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // Handle polling errors if necessary
    }
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message.Type == MessageType.Text)
        {
            try
            {
                var userId = update.Message.Chat.Id;
                string Sendermessage = null;
                string userQuery = update.Message.Text;
                string apiUrl = $"http://en.wikipedia.org/w/api.php?action=query&format=json&prop=extracts&exintro=true&titles={userQuery.Replace(" ", "%30")}";

                using (WebClient webClient = new WebClient())
                {
                    string response = webClient.DownloadString(apiUrl);
                    var wikipediaResponse = JsonConvert.DeserializeObject<WikipediaResponse>(response);

                    if (wikipediaResponse != null && wikipediaResponse.Query != null && wikipediaResponse.Query.Pages != null)
                    {
                        var firstPage = wikipediaResponse.Query.Pages.Values.First();
                        if (firstPage != null)
                        {
                            string title = firstPage.Title;
                            string extract = firstPage.Extract;

                            extract = RemoveHtmlElements(extract);

                            Sendermessage = (extract);
                        }
                        else
                        {
                            Sendermessage = "No information found for the given query.";
                        }
                    }
                    else
                    {
                        Sendermessage = "No information found for the given query.";
                    }
                }

                if (string.IsNullOrEmpty(Sendermessage))
                {
                    Sendermessage = "No information found for the given query.";
                }
                else
                {
                    Sendermessage = $"Information about {update.Message.Text}: {Sendermessage}";
                }

                Console.WriteLine($"Received message from user {userId}: {userQuery}");
                await botClient.SendTextMessageAsync(userId, Sendermessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }


    static string RemoveHtmlElements(string input)
    {
        string pattern = @"<[^>]+>";
        return Regex.Replace(input, pattern, String.Empty);
    }
}
public class WikipediaResponse
{
    public Query Query { get; set; }
}

public class Query
{
    public Dictionary<string, Page> Pages { get; set; }
}

public class Page
{
    public string Title { get; set; }
    public string Extract { get; set; }
}

