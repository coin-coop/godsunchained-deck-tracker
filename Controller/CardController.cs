using GodsUnchained_Deck_Tracker.Model.Entities;
using GodsUnchained_Deck_Tracker.Utilities.Http;
using Imazen.WebP;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GodsUnchained_Deck_Tracker.Controller
{
    public class CardController : AccessController
    {
        private static CardController instance = null;
        private static readonly object mutex = new object();

        private static readonly string cardsFilePath = projectDirectory + "\\..\\Resources\\Files\\cards.txt";
        //private static readonly string cardsFilePath = projectDirectory + "\\Resources\\Files\\cards.txt";

        private static List<string> cards;

        public static CardController GetInstance {
            get {
                lock (mutex) {
                    if (instance == null) {
                        instance = new CardController();
                    }
                    return instance;
                }
            }
        }

        public static async void GetCards(string userAddress) {
            cards = new List<string>();

            Task<int> totalRecordsTask = GetTotalRecords($"user/{userAddress}/inventory");
            await totalRecordsTask;
            int total = totalRecordsTask.Result;

            if (!File.Exists(cardsFilePath)) {
                File.Create(cardsFilePath).Dispose();
            }
            
            ReadCardsFromAPI(userAddress, total);
        }

        public static async Task<List<CardView>> GetCardViews() {
            StreamReader cardsReader = new StreamReader(cardsFilePath);
            List<CardView> cardViews = new List<CardView>();

            string line;
            while ((line = cardsReader.ReadLine()) != null) {
                string[] splitCard = line.Split("::");
                string cardId = splitCard[1];

                Task<CardView> cardViewTask = GetCardView(cardId);
                await cardViewTask;
                cardViews.Add(cardViewTask.Result);
            }

            return cardViews;
        }

       private static async Task<CardView> GetCardView(string cardId) {
            Card card = new Card("", cardId, true);
            CardView cardView = new CardView(card);

            Task<BitmapImage> loadImageTask = LoadWebP($"https://card.godsunchained.com/?id={card.Prototype.Id}&q=4&w=256");
            await loadImageTask;
            cardView.Image = loadImageTask.Result;

            return cardView;
        }

        private static async Task<BitmapImage> LoadWebP(string url) {
            var httpClient = new HttpClient();
            var buffer = await httpClient.GetByteArrayAsync(url);
            var decoder = new SimpleDecoder();
            var bitmap = decoder.DecodeFromBytes(buffer, buffer.Length);
            var bitmapImage = new BitmapImage();

            using (var stream = new MemoryStream()) {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        private static async void ReadCardsFromAPI(string userAddress, int total) {
            int page = 1;
            int perPage = 40;
            do {
                Task<string> getPrototypesTask = RestClient.Get<string>($"user/{userAddress}/inventory?page={page}&perPage={perPage}");
                await getPrototypesTask;
                string prototypesResult = getPrototypesTask.Result;
                page++;

                JObject prototypesResultObject = JObject.Parse(prototypesResult);
                JArray records = (JArray) prototypesResultObject.GetValue("records");

                foreach (JToken record in records) {
                    AddCard(record);
                }

            } while (page * perPage <= total + perPage - 1);

            int i = 0;
            StreamWriter cardWriter = new StreamWriter(cardsFilePath);
            foreach (string card in cards) {
                i++;
                cardWriter.WriteLine(i.ToString() + "::" + card);
            }
            cardWriter.Flush();
            cardWriter.Close();
        }

        private static void AddCard(JToken record) {
            cards.Add(((int) record["proto"]).ToString() + "::" + ((int) record["purity"]).ToString());
        }
    }
}
