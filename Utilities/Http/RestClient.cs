using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace GodsUnchained_Deck_Tracker.Utilities.Http
{
    public static class RestClient
    {
        private static readonly string apiBasicUri = "https://api.godsunchained.com/v0/";
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> Get<T>(string url) {
            string callResult = "";
            try {
                if(client.BaseAddress == null) {
                    client.BaseAddress = new Uri(apiBasicUri);
                }
                var result = await client.GetAsync(url);
                result.EnsureSuccessStatusCode();
                callResult = await result.Content.ReadAsStringAsync();
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }
            return callResult;
        }
    }
}
