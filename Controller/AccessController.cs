using GodsUnchained_Deck_Tracker.Utilities.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace GodsUnchained_Deck_Tracker.Controller
{
    public class AccessController
    {
        public static async Task<int> GetTotalRecords(string path) {
            Task<string> cardsTask = RestClient.Get<string>($"{path}");
            await cardsTask;

            string cardsResult = cardsTask.Result;

            JObject prototypesObject = JObject.Parse(cardsResult);
            return (int) prototypesObject.GetValue("total");
        }
    }
}
