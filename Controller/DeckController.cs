using GodsUnchained_Companion_App.Model.Entities;
using GodsUnchained_Companion_App.Utilities.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsUnchained_Companion_App.Controller
{
    public class DeckController : AccessController
    {
        private static DeckController instance = null;
        private static readonly object mutex = new object();

        private static readonly string decksFilePath = projectDirectory + "\\..\\Resources\\Files\\decks.txt";
        private static readonly string matchesFilePath = projectDirectory + "\\..\\Resources\\Files\\matches.txt";
        //private static readonly string decksFilePath = projectDirectory + "\\Resources\\Files\\decks.txt";
        //private static readonly string matchesFilePath = projectDirectory + "\\Resources\\Files\\matches.txt";

        private static Dictionary<string, string> matches;

        public static DeckController GetInstance {
            get {
                lock (mutex) {
                    if (instance == null) {
                        instance = new DeckController();
                    }
                    return instance;
                }
            }
        }

        public static Deck GetDeck(string name, string god, string cards, string games) {
            return new Deck(name, god, GetCards(cards), Int32.Parse(games));
        }

        public static List<Deck> GetDecks() {
            StreamReader decksReader = new StreamReader(decksFilePath);
            List<Deck> decks = new List<Deck>();

            int i = 0;
            string line;
            while ((line = decksReader.ReadLine()) != null) {
                string[] splitDeck = line.Split("::");
                string god = splitDeck.First();
                string cards = splitDeck[1];
                string games = splitDeck.Last();

                i++;

                decks.Add(GetDeck("Deck " + i.ToString(), god, cards, games));
            }
            decksReader.Close();

            return decks;
        }

        /**
         * All decks played since 29.06.2020
         **/
        public static async void GetDecksPlayedByPlayer() {
            matches = new Dictionary<string, string>();

            string timestampBegin = new DateTimeOffset(DateTime.UtcNow.AddDays(-30)).ToUnixTimeSeconds().ToString();
            string timestampEnd = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();

            Task<int> totalRecordsTask = GetTotalRecords($"match?start_time={timestampBegin}-{timestampEnd}");
            await totalRecordsTask;
            int total = totalRecordsTask.Result;

            if (!File.Exists(matchesFilePath)) {
                File.Create(matchesFilePath).Dispose();
            }

            if (File.ReadLines(matchesFilePath).Count() < total) {
                ReadMatchesFromAPI(total, timestampBegin, timestampEnd);
            }
        }

        public static void ImportDeckFromCode(string deckCode) {
            string[] deck = deckCode.Split(new[] { ',' }, 2);
            SaveDeckToFile(deck.First(), "", deck.Last(), "");
        }

        private static async void ReadMatchesFromAPI(int total, string timestampBegin, string timestampEnd) {
            int page = 1;
            int perPage = 100;
            do {
                Task<string> getMatchesTask = RestClient.Get<string>($"match?start_time={timestampBegin}-{timestampEnd}&page={page}&perPage={perPage}");
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
                    SaveDeckToFile(god, godPower, cardsStr, endTime);
                }

                value += cardsStr;

                matches[(string) record["game_id"]] = value;
            }
        }

        private static void SaveDeckToFile(string god, string godPower, string cardsStr, string endTime) {
            string currentDeck = god + "::" + cardsStr;

            if (!File.ReadAllText(decksFilePath).Contains(currentDeck)) {
                StreamWriter decksWriter = File.AppendText(decksFilePath);
                decksWriter.WriteLine(currentDeck + "::" + godPower + "::" + endTime + "::1");
                decksWriter.Close();
            } else {
                List<string> decks = File.ReadAllLines(decksFilePath).ToList();

                for (int i = decks.Count - 1; i >= 0; --i) {
                    string newDeck = currentDeck + "::" + godPower;
                    if(decks[i].Contains(currentDeck)) {
                        Debug.WriteLine("Replace deck: " + decks[i]);
                        string[] data = decks[i].Split("::");

                        decks.RemoveAt(i);

                        int countGames = Int32.Parse(data.Last());

                        long currentEndTime = long.Parse(endTime);
                        long foundEndTime = long.Parse(data[2]);

                        if (foundEndTime >= currentEndTime) {
                            newDeck = newDeck + "::" + foundEndTime;
                        } else {
                            newDeck = newDeck + "::" + currentEndTime;
                        }

                        decks.Add(newDeck + "::" + ++countGames);
                        Debug.WriteLine("Replaced deck: " + newDeck + "::" + countGames);
                        break;
                    }
                }
                File.WriteAllLines(decksFilePath, decks);
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
