using GodsUnchained_Deck_Tracker.Model.Entities;
using GodsUnchained_Deck_Tracker.Utilities.Http;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GodsUnchained_Deck_Tracker.Controller
{
    public static class UserController
    {
        private static User user;
        private static readonly string logFilePath = Properties.Settings.Default.logFilePath;

        public static User GetUser() {
            return user;
        }

        public static User GetUser(string userData, string userRank) {
            if (userData.Contains("records")) {
                JObject userObject = JObject.Parse(userData);
                JArray records = (JArray) userObject.GetValue("records");

                if (records.Count == 1) {
                    JToken result = records.FirstOrDefault();
                    user.Level = (int) result["xp_level"];
                    user.TotalExp = (int) result["total_xp"];
                    user.ExpToNextLevel = (int) result["xp_to_next"];
                    user.Wins = (int) result["won_matches"];
                    user.Loses = (int) result["lost_matches"];
                    user.Name = (string) result["username"];
                }
            }

            if (userRank.Contains("records")) {
                JObject userObject = JObject.Parse(userRank);
                JArray records = (JArray) userObject.GetValue("records");

                foreach(JToken record in records) {
                    if ((int) record["game_mode"] == 13) {
                        user.Rating = (int) record["rating"];
                        user.Rank = GetRank((int) record["rank_level"]);
                        user.WinPoints = (int) record["win_points"];
                        user.LossPoints = (int) record["loss_points"];
                    }
                }
            }
            return user;
        }

        public async static Task<string> GetUserDataAsync() {
            ReadUserIdAndInitalizeUser();

            return await RestClient.Get<string>($"properties?user_id={user.Id}");
        }

        public async static Task<string> GetUserRankAsync() {
            ReadUserIdAndInitalizeUser();

            return await RestClient.Get<string>($"rank?user_id={user.Id}");
        }

        private static void ReadUserIdAndInitalizeUser() {
            if(user == null || user.Id == 0) {
                FileStream logFileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader logReader = new StreamReader(logFileStream);

                int id = 0;

                string searchFor = "Sending RegisterPlayer msg... apolloID:";
                string line;
                while ((line = logReader.ReadLine()) != null) {
                    if (line.Contains(searchFor)) {
                        string[] splitStr = line.Split(searchFor);
                        id = Int32.Parse(splitStr.Last());
                        break;
                    }
                }

                logReader.Close();
                logFileStream.Close();

                user = new User {
                    Id = id
                };
            }
        }

        private static string GetRank(int rank) {
            return string.Concat(Enum.GetName(typeof(Rank), (int) rank).Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        }
    }
}
