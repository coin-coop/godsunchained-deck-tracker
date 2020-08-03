using GodsUnchained_Deck_Tracker.Controller;
using GodsUnchained_Deck_Tracker.Model.Entities;
using GodsUnchained_Deck_Tracker.Model.Enums;
using GodsUnchained_Deck_Tracker.Utilities;
using GodsUnchained_Deck_Tracker.Utilities.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsUnchained_Deck_Tracker.Tracker
{
    public static class DeckManager
    {
        private static int cardsInDeck = 30;

        private static Dictionary<string, string> matches;

        private static SortedDictionary<Card, int> sortedDeckCards;
        private static SortedDictionary<Card, int> sortedDrawnCards;
        private static SortedDictionary<Card, int> sortedExtraDrawnCards;
        private static SortedDictionary<Card, int> sortedSanctumDrawnCards;

        private static readonly string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
        //private static readonly string projectDirectory = Environment.CurrentDirectory;
        private static readonly string decksFilePath = projectDirectory + "\\..\\Resources\\Files\\decks.txt";
        private static readonly string matchesFilePath = projectDirectory + "\\..\\Resources\\Files\\matches.txt";
        //private static readonly string decksFilePath = projectDirectory + "\\Resources\\Files\\decks.txt";
        //private static readonly string matchesFilePath = projectDirectory + "\\Resources\\Files\\matches.txt";
        private static readonly string logFilePath = Properties.Settings.Default.logFilePath;

        public static int GetCardsInDeck() {
            return cardsInDeck;
        }

        public static List<CardView> GetCardsView(List<Card> cards) {
            List<Card> sortedCards = cards.OrderBy(o => o.Prototype.Mana).ThenBy(o => o.Prototype.Name).ToList();

            List<CardView> cardsView = new List<CardView>();

            foreach (Card card in sortedCards) {
                int index = cardsView.FindIndex(item => item.Name.Trim() == card.Prototype.Name);
                if (index >= 0) {
                    CardView cardView = cardsView[index];
                    cardView.Amount += 1;
                    cardsView[index] = cardView;
                } else {
                    cardsView.Add(new CardView(card));
                }
            }
            return cardsView;
        }

        public static List<CardView> GetCurrentDeckCards() {
            return ConvertDictionaryToCardViewList(sortedDeckCards, true);
        }

        public static List<Card> GetDeckCards(Deck deck) {
            return deck.Cards.OrderBy(o => o.Prototype.Mana).ThenBy(o => o.Prototype.Name).ToList();
        }

        public static List<CardView> GetExtraDrawnCards() {
            return ConvertDictionaryToCardViewList(sortedExtraDrawnCards, false);
        }

        public static List<CardView> GetSanctumDrawnCards() {
            return ConvertDictionaryToCardViewList(sortedSanctumDrawnCards, false);
        }

        public static string GetDeckName() {
            FileStream logFileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader logReader = new StreamReader(logFileStream);

            string searchFor = "Loaded deck";
            string line;
            while ((line = logReader.ReadLine()) != null) {
                if (line.Contains(searchFor)) {
                    string[] splitStr = line.Split(searchFor);
                    return splitStr.Last().Trim().Split("for player").First();
                }
            }

            logReader.Close();
            logFileStream.Close();

            return "";
        }

        public static List<Deck> GetDecks() {
            StreamReader decksReader = new StreamReader(decksFilePath);
            List<Deck> decks = new List<Deck>();

            int i = 0;
            string line;
            while ((line = decksReader.ReadLine()) != null) {
                string[] splitDeck = line.Split("::");
                string god = splitDeck.First();
                string cards = splitDeck.Last();
                string[] cardIds = cards.Split(",");
                cardIds = cardIds.Take(cardIds.Count() - 1).ToArray();

                List<Card> cardList = new List<Card>();
                foreach (string cardId in cardIds) {
                    Card card = new Card("", cardId, true);
                    cardList.Add(card);
                }

                i++;
                decks.Add(new Deck("Deck " + i.ToString(), god, cardList));
            }

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

        private static List<CardView> ConvertDictionaryToCardViewList(SortedDictionary<Card, int> cardsDicitionary, bool deckCards) {
            List<Card> cards = new List<Card>();

            if (cardsDicitionary != null) {
                foreach (KeyValuePair<Card, int> deckCard in cardsDicitionary) {
                    if (deckCard.Value == 2) {
                        cards.Add(deckCard.Key);
                        cards.Add(new Card(deckCard.Key));
                    } else if (deckCard.Value == 1) {
                        cards.Add(deckCard.Key);
                    }
                }

                List<CardView> cardsView = GetCardsView(cards);

                if (deckCards) {
                    foreach (KeyValuePair<Card, int> deckCard in sortedDeckCards) {
                        if (deckCard.Value < 1) {
                            CardView cardView = new CardView(deckCard.Key) {
                                Amount = 0
                            };
                            cardsView.Add(cardView);
                            Debug.WriteLine(cardView.Name + cardView.Mana + cardView.Amount.ToString());
                        }
                    }
                }
                return cardsView.OrderBy(o => o.Mana).ThenBy(o => o.Name).ToList();
            }
            
            return new List<CardView>();
        }

        public static void UpdateDeck(Deck selectedDeck) {
            Dictionary<Card, int> drawnCards = new Dictionary<Card, int>();
            Dictionary<Card, int> sanctumDrawnCards = new Dictionary<Card, int>();

            cardsInDeck = 30;

            if(File.Exists(logFilePath)) {
                FileStream logFileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader logReader = new StreamReader(logFileStream);

                string line;
                while ((line = logReader.ReadLine()) != null) {
                    if (line.Contains("Deck to Hand") || line.Contains("None to Hand")) {
                        string[] splitStr = line.Split("EffectSolver.MoveCard - ");
                        string secondStr = splitStr.Last();
                        if (line.Contains("Deck to Hand")) {
                            cardsInDeck -= 1;
                            UpdateDrawnCards(secondStr, " moved from Deck to Hand", drawnCards);
                        } else {
                            UpdateDrawnCards(secondStr, " moved from None to Hand", sanctumDrawnCards);
                        }
                    }
                }

                logReader.Close();
                logFileStream.Close();
            }

            sortedDeckCards = new SortedDictionary<Card, int>();
            sortedDrawnCards = new SortedDictionary<Card, int>(drawnCards);
            sortedSanctumDrawnCards = new SortedDictionary<Card, int>(sanctumDrawnCards);

            if(selectedDeck != null) {
                foreach (Card deckCard in selectedDeck.Cards) {
                    if (sortedDeckCards.ContainsKey(deckCard)) {
                        sortedDeckCards[deckCard] = 2;
                    } else {
                        sortedDeckCards[deckCard] = 1;
                    }
                }
            }

            foreach (KeyValuePair<Card, int> drawnCard in sortedDrawnCards) {
                foreach (KeyValuePair<Card, int> deckCard in sortedDeckCards) {
                    if (drawnCard.Key.Prototype.Name == deckCard.Key.Prototype.Name) {
                        sortedDeckCards[deckCard.Key] = deckCard.Value - drawnCard.Value;
                        break;
                    }
                }
            }

            sortedExtraDrawnCards = new SortedDictionary<Card, int>();

            foreach (KeyValuePair<Card, int> drawnCard in sortedDrawnCards) {
                if (!sortedDeckCards.ContainsKey(drawnCard.Key)) {
                    //TODO: later cards which are shuffling in should increase amount of cards
                    cardsInDeck += 1;
                    if (!sortedExtraDrawnCards.ContainsKey(drawnCard.Key)) {
                        sortedExtraDrawnCards[drawnCard.Key] = 1;
                    } else {
                        int cardCount = sortedExtraDrawnCards[drawnCard.Key];
                        sortedExtraDrawnCards[drawnCard.Key] = cardCount + 1;
                    }
                }
            }
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
            //Debug.WriteLine(value);

            JArray playerInfo = (JArray) record["player_info"];

            foreach (JToken player in playerInfo) {
                //Debug.WriteLine(player);
                int userID = (int) player["user_id"];
                string god = (string) player["god"];
                string godPower = ((int) player["god_power"]).ToString();
                JArray cards = (JArray) player["cards"];

                value += userID.ToString() + ";;" + god + ";;";

                string cardsStr = "";
                foreach (JToken card in cards) {
                    cardsStr += card + ",";
                }

                if (UserController.GetUser().Id == userID && !File.ReadAllText(decksFilePath).Contains(god + "::" + cardsStr)) {
                    //add check if deck is not already inserted
                    StreamWriter decksWriter = File.AppendText(decksFilePath);
                    decksWriter.WriteLine(god + "::" + cardsStr);
                    decksWriter.Close();
                }

                value += cardsStr;

                //Debug.WriteLine(record["game_id"]);
                matches[(string) record["game_id"]] = value;
            }
        }

        private static void UpdateDrawnCards(string toSplit, string splitFor, Dictionary<Card, int> drawnCards) {
            string[] splitStr = toSplit.Split(splitFor);
            string cardName = splitStr[0];
            if (drawnCards.ContainsKey(new Card(cardName))) {
                Card key = drawnCards.First(drawnCard => drawnCard.Key.Prototype.Name == cardName).Key;
                drawnCards[key] = drawnCards[key] + 1;
            } else {
                drawnCards[new Card(cardName)] = 1;
            }
        }
    }
}
