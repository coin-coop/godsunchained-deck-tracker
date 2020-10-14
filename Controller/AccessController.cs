using GodsUnchained_Companion_App.Utilities.Http;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GodsUnchained_Companion_App.Controller
{
    public class AccessController
    {

        protected static readonly string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
        //protected static readonly string projectDirectory = Environment.CurrentDirectory;

        public static async Task<int> GetTotalRecords(string path) {
            Task<string> cardsTask = RestClient.Get<string>($"{path}");
            await cardsTask;

            string cardsResult = cardsTask.Result;

            JObject prototypesObject = JObject.Parse(cardsResult);
            return (int) prototypesObject.GetValue("total");
        }
    }
}
