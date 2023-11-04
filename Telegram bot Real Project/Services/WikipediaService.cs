using System;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram_bot_Real_Project.Interfaces;

public class WikipediaService : IWikipediaService
{
    private readonly HttpClient _httpClient;

    public WikipediaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetWikipediaSummaryAsync(string query)
    {
        try
        {
            string apiUrl = $"http://en.wikipedia.org/w/api.php?action=query&format=json&prop=extracts&exintro=true&titles={query.Replace(" ", "%20")}";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var wikipediaResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<WikipediaResponse>(responseBody);

            if (wikipediaResponse?.Query?.Pages != null && wikipediaResponse.Query.Pages.Count > 0)
            {
                var firstPage = wikipediaResponse.Query.Pages.Values.GetEnumerator().Current;
                if (firstPage != null)
                {
                    string extract = RemoveHtmlTags(firstPage.Extract);
                    Console.WriteLine($"Extract: {extract}");
                    return extract;
                }
            }

            return "No information found for the given query.";
        }
        catch (Exception ex)
        {
            return $"Error occurred: {ex.Message}";
        }
    }

    private string RemoveHtmlTags(string input)
    {
        // Remove HTML tags and their attributes using a regular expression
        return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", String.Empty);
    }

    private class WikipediaResponse
    {
        public Query Query { get; set; }
    }

    private class Query
    {
        public Dictionary<string, Page> Pages { get; set; }
    }

    private class Page
    {
        public string Title { get; set; }
        public string Extract { get; set; }
    }
}
