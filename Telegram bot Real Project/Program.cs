
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram_bot_Real_Project.Interfaces;

namespace Telegram_bot_Real_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient<IWikipediaService, WikipediaService>();
            builder.Services.AddSingleton<IUpdateHandler,MessageHandler>();
            TelegramBotClient client = new TelegramBotClient("6824038704:AAFuVOS7wJlKsCTAkJtVrqZSZXCOhPZQpwI");

            var serviceProv = builder.Services.BuildServiceProvider();
            var messageHandler = new MessageHandler(serviceProv.GetRequiredService<IWikipediaService>());

            client.StartReceiving(messageHandler);
            builder.Services.AddSingleton(client);
            //builder.Services.AddSingleton(new TelegramBotClient("6824038704:AAFuVOS7wJlKsCTAkJtVrqZSZXCOhPZQpwI"));
            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}