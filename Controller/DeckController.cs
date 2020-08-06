using GodsUnchained_Deck_Tracker.Model.Entities;
using GodsUnchained_Deck_Tracker.Utilities.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsUnchained_Deck_Tracker.Controller
{
    public static class DeckController
    {
        private static readonly string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
        //private static readonly string projectDirectory = Environment.CurrentDirectory;
        private static readonly string decksFilePath = projectDirectory + "\\..\\Resources\\Files\\decks.txt";
        private static readonly string matchesFilePath = projectDirectory + "\\..\\Resources\\Files\\matches.txt";
        //private static readonly string decksFilePath = projectDirectory + "\\Resources\\Files\\decks.txt";
        //private static readonly string matchesFilePath = projectDirectory + "\\Resources\\Files\\matches.txt";

        private static Dictionary<string, string> matches;

        public static List<Deck> GetDecks() {
            StreamReader decksReader = new StreamReader(decksFilePath);
            List<Deck> decks = new List<Deck>();

            int i = 0;
            string line;
            while ((line = decksReader.ReadLine()) != null) {
                string[] splitDeck = line.Split("::");
                string god = splitDeck.First();
                string cards = splitDeck.Last();

                List<Card> cardList = GetCards(cards);

                i++;
                decks.Add(new Deck("Deck " + i.ToString(), god, cardList));
            }
            decksReader.Close();

            return decks;
        }

        /**
         * All decks played since 29.06.2020
         **/
        public static async void GetDecksPlayedByPlayer() {
            matches = new Dictionary<string, string>();
            string timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();

            Task<string> matchesTask = RestClient.Get<string>($"match?start_time=1593474160-{timestamp}");
            await matchesTask;

            string matchesResult = matchesTask.Result;

            JObject prototypesObject = JObject.Parse(matchesResult);
            int total = (int) prototypesObject.GetValue("total");

            if (!File.Exists(matchesFilePath)) {
                File.Create(matchesFilePath).Dispose();
            }

            if (File.ReadLines(matchesFilePath).Count() < total) {
                ReadMatchesFromAPI(total, timestamp);
            }
        }

        public static void ImportDeckFromCode(string deckCode) {
            string[] deck = deckCode.Split(new[] { ',' }, 2);
            SaveDeckToFile(deck.First(), deck.Last());
        }

        private static async void ReadMatchesFromAPI(int total, string timestamp) {
            int page = 1;
            int perPage = 100;
            do {
                Task<string> getMatchesTask = RestClient.Get<string>($"match?start_time=1593474160-{timestamp}&page={page}&perPage={perPage}");
                await getMatchesTask;
                string matchesResult = getMatchesTask.Result;
                page++;

                JObject matchesResultObject = JObject.Parse(matchesResult);
                JArray records = (JArray) matchesResultObject.GetValue("records");

                foreach (JToken record in records) {
                    int winnerId = (int) record["player_won"];
                    int loserId = (int) record["player_lost"];
                    int gameMode = (int) record["game_mode"];
                    if (gameMode == 13 && (UserController.GetUser().Id == winnerId || UserController.GetUser().Id == loserId)) {
                        AddMatch(record);
                    }
                    AddMatch(record);
                }

            } while (page * perPage <= total + perPage - 1);

            StreamWriter matchWriter = new StreamWriter(matchesFilePath);
            foreach (KeyValuePair<string, string> match in matches) {
                matchWriter.WriteLine(match.Key + "::" + match.Value);
            }
            matchWriter.Flush();
            matchWriter.Close();
        }

        private static void AddMatch(JToken record) {
            string winnerId = ((int) record["player_won"]).ToString();
            string loserId = ((int) record["player_lost"]).ToString();
            string gameMode = ((int) record["game_mode"]).ToString();
            string startTime = ((int) record["start_time"]).ToString();
            string endTime = ((int) record["end_time"]).ToString();
            string totalRounds = ((int) record["total_rounds"]).ToString();

            string value = winnerId + "::" + loserId + "::" + gameMode + "::" + startTime + "::" + endTime + "::" + totalRounds + "::";

            JArray playerInfo = (JArray) record["player_info"];

            foreach (JToken player in playerInfo) {
                int userID = (int) player["user_id"];
                string god = (string) player["god"];
                string godPower = ((int) player["god_power"]).ToString();
                JArray cards = (JArray) player["cards"];

                value += userID.ToString() + ";;" + god + ";;";

                string cardsStr = "";
                foreach (JToken card in cards) {
                    cardsStr += card + ",";
                }
                if(cardsStr.Length > 1) {
                    cardsStr = cardsStr.Remove(cardsStr.Length - 1);
                }

                if (UserController.GetUser().Id == userID) {
                    SaveDeckToFile(god, cardsStr);
                }

                value += cardsStr;

                matches[(string) record["game_id"]] = value;
            }
        }

        private static void SaveDeckToFile(string god, string cardsStr) {
            if (!File.ReadAllText(decksFilePath).Contains(god + "::" + cardsStr)) {
                StreamWriter decksWriter = File.AppendText(decksFilePath);
                decksWriter.WriteLine(god + "::" + cardsStr);
                decksWriter.Close();
            }
        }

        private static List<Card> GetCards(string cards) {
            string[] cardIds = cards.Split(",");

            List<Card> cardList = new List<Card>();

            foreach (string cardId in cardIds) {
                if (cardId != "") {
                    Card card = new Card("", cardId, true);
                    cardList.Add(card);
                }
            }

            return cardList;
        }
    }
}
