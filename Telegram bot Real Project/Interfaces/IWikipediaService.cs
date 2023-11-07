namespace Telegram_bot_Real_Project.Interfaces
{
    public interface IWikipediaService
    {
       Task <string> GetWikipediaSummaryAsync(string query,string language);
    }
}
