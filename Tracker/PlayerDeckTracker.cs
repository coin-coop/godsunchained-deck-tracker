using GodsUnchained_Deck_Tracker.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GodsUnchained_Deck_Tracker.Tracker
{
    public sealed class PlayerDeckTracker : DeckTracker
    {
        private static PlayerDeckTracker instance = null;
        private static readonly object mutex = new object();

        public Deck SelectedDeck { get; set; }

        private static readonly string logFilePath = Properties.Settings.Default.logFilePath;

        private static List<Card> currentDeckCards;
        private static List<Card> extraDrawnCards;
        private static List<Card> sanctumDrawnCards;

        PlayerDeckTracker() {
            CardsInDeck = 30;
        }

        public static PlayerDeckTracker GetInstance {
            get {
                lock (mutex) {
                    if (instance == null) {
                        instance = new PlayerDeckTracker();
                    }
                    return instance;
                }
            }
        }

        public List<CardView> GetSelectedDeckCards() {
            return GetDeckCards(SelectedDeck);
        }

        public List<CardView> GetDeckCards(Deck deck) {
            return GetCardsView(deck.Cards);
        }

        public List<CardView> GetCurrentDeckCards() {
            return GetCardsView(currentDeckCards, SelectedDeck, true);
        }

        public List<CardView> GetExtraDrawnCards() {
            return GetCardsView(extraDrawnCards);
        }

        public List<CardView> GetSanctumDrawnCards() {
            return GetCardsView(sanctumDrawnCards);
        }

        public string GetDeckName() {
            FileStream logFileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader logReader = new StreamReader(logFileStream);

            string searchFor = "Loaded deck";
            string line;
            while ((line = logReader.ReadLine()) != null) {
                if (line.Contains(searchFor)) {
                    return line.Split(searchFor).Last().Trim().Split("for player").First();
                }
            }

            logReader.Close();
            logFileStream.Close();

            return "";
        }

        public void UpdateDeck() {
            List<Card> drawnCards = new List<Card>();

            currentDeckCards = new List<Card>(SelectedDeck.Cards);
            extraDrawnCards = new List<Card>();
            sanctumDrawnCards = new List<Card>();

            CardsInDeck = 30;

            if (File.Exists(logFilePath)) {
                FileStream logFileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader logReader = new StreamReader(logFileStream);

                string line;
                while ((line = logReader.ReadLine()) != null) {
                    if (line.Contains("Deck to Hand") || line.Contains("None to Hand")) {
                        string found = line.Split("EffectSolver.MoveCard - ").Last();
                        if (line.Contains("Deck to Hand")) {
                            CardsInDeck -= 1;
                            UpdateDrawnCards(found, " moved from Deck to Hand", drawnCards);
                        } else {
                            UpdateDrawnCards(found, " moved from None to Hand", sanctumDrawnCards);
                        }
                    }
                }

                logReader.Close();
                logFileStream.Close();
            }

            foreach (Card drawnCard in drawnCards) {
                if (currentDeckCards.Exists(item => item.Prototype.Name == drawnCard.Prototype.Name)) {
                    int index = currentDeckCards.FindIndex(item => item.Prototype.Name == drawnCard.Prototype.Name);
                    currentDeckCards.RemoveAt(index);
                } else if (!SelectedDeck.Cards.Exists(item => item.Prototype.Name == drawnCard.Prototype.Name)) {
                    //TODO: later cards which are shuffling in should increase amount of cards
                    CardsInDeck += 1;
                    extraDrawnCards.Add(drawnCard);
                }
            }
        }

        private void UpdateDrawnCards(string toSplit, string splitFor, List<Card> drawnCards) {
            drawnCards.Add(new Card(toSplit.Split(splitFor)[0], true));
        }
    }
}
