using GodsUnchained_Deck_Tracker.Model.Entities;
using GodsUnchained_Deck_Tracker.Utilities.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GodsUnchained_Deck_Tracker.Controller
{
    public class CardPrototypeController : AccessController
    {
        private static CardPrototypeController instance = null;
        private static readonly object mutex = new object();

        private static readonly string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
        private static readonly string protytpesFilePath = projectDirectory + "\\..\\Resources\\Files\\prototypes.txt";
        //private static readonly string projectDirectory = Environment.CurrentDirectory;
        //private static readonly string protytpesFilePath = projectDirectory + "\\Resources\\Files\\prototypes.txt";
        private static Dictionary<string, string> prototypes;

        public static CardPrototypeController GetInstance {
            get {
                lock (mutex) {
                    if (instance == null) {
                        instance = new CardPrototypeController();
                    }
                    return instance;
                }
            }
        }

        public static Prototype GetPrototypeByName(string name) {
            return GetPrototype(name);
        }

        public static Prototype GetPrototypeById(string id) {
            return GetPrototype("::" + id + "::");
        }

        public async static void GetPrototypesAsync() {
            prototypes = new Dictionary<string, string>();

            Task<int> totalRecordsTask = GetTotalRecords($"proto");
            await totalRecordsTask;
            int total = totalRecordsTask.Result;

            if(!File.Exists(protytpesFilePath)) {
                File.Create(protytpesFilePath).Dispose();
            }

            if (File.ReadLines(protytpesFilePath).Count() < total) {
                ReadPrototypesFromAPI(total);
            }
        }

        private static Prototype GetPrototype(string searchFor) {
            StreamReader protytpesReader = new StreamReader(protytpesFilePath);

            string line;
            while ((line = protytpesReader.ReadLine()) != null) {
                if (line.Contains(searchFor)) {
                    return new Prototype(line.Split("::"));
                }
            }

            //TODO: handle case for not found prototype
            return null;
        }

        private static async void ReadPrototypesFromAPI(int total) {
            int page = 1;
            int perPage = 40;
            do {
                Task<string> getPrototypesTask = RestClient.Get<string>($"proto?page={page}&perPage={perPage}");
                await getPrototypesTask;
                string prototypesResult = getPrototypesTask.Result;
                page++;

                JObject prototypesResultObject = JObject.Parse(prototypesResult);
                JArray records = (JArray) prototypesResultObject.GetValue("records");

                foreach (JToken record in records) {
                    AddPrototype(record);
                }

            } while (page * perPage <= total + perPage - 1);

            StreamWriter prototypeWriter = new StreamWriter(protytpesFilePath);
            foreach (KeyValuePair<string, string> prototype in prototypes) {
                prototypeWriter.WriteLine(prototype.Key + "::" + prototype.Value);
            }
            prototypeWriter.Flush();
            prototypeWriter.Close();
        }

        private static void AddPrototype(JToken record) {
            string value = ((int) record["id"]).ToString() + "::" + (string) record["effect"] + "::" + (string) record["god"] + "::" + (string) record["rarity"] + "::" + ((int) record["mana"]).ToString() + "::" + (string) record["type"] + "::" + (string) record["set"];
            prototypes[(string) record["name"]] = value;
        }
    }
}
