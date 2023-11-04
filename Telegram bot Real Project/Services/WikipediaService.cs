using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
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
        HttpClient client = new HttpClient();
        try
        {
            string apiUrl = $"http://en.wikipedia.org/w/api.php?action=query&format=json&prop=extracts&exintro=true&titles={WebUtility.UrlEncode(query)}";
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var wikipediaResponse = JsonConvert.DeserializeObject<WikipediaResponse>(responseBody);

            if (wikipediaResponse?.Query?.Pages != null && wikipediaResponse.Query.Pages.Count > 0)
            {
                var firstPage = wikipediaResponse.Query.Pages.Values.First();
                if (firstPage != null)
                {
                    string extract = RemoveHtmlTags(firstPage.Extract);
                    return($"Extract: {extract}");
                }
            }
                return ("No information found for the given query.");
        }
        catch (Exception ex)
        {
            return($"Error occurred: {ex.Message}");
        }
    }

    private static string RemoveHtmlTags(string input)
    {
        string pattern = @"<[^>]+>";
        return Regex.Replace(input, pattern, String.Empty);
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
