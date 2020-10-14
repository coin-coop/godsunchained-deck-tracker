using GodsUnchained_Companion_App.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsUnchained_Companion_App.Tracker
{
    public class DeckTracker : IDeckTracker
    {
        protected static readonly string logFilePath = Properties.Settings.Default.logFilePath;

        public int CardsInDeck { get; set; }

        public List<CardView> GetCardsView(List<Card> cards, Deck selectedDeck = null, bool currentCards = false) {
            List<Card> sortedCards = cards.OrderBy(o => o.Prototype.Mana).ThenBy(o => o.Prototype.Name).ToList();

            List<CardView> cardsView = new List<CardView>();

            foreach (Card card in sortedCards) {
                int index = cardsView.FindIndex(item => item.Name == card.Prototype.Name);
                if (index >= 0) {
                    CardView cardView = cardsView[index];
                    cardView.Amount += 1;
                    cardView.DrawProbability = GetDrawProbability(cardView);
                    cardsView[index] = cardView;
                } else {
                    CardView cardView = new CardView(card);
                    cardView.DrawProbability = cardView.DrawProbability = GetDrawProbability(cardView);
                    cardsView.Add(cardView);
                }
            }

            if (currentCards) {
                foreach (Card card in selectedDeck.Cards) {
                    if (!sortedCards.Exists(item => item.Prototype.Name == card.Prototype.Name)) {
                        int index = cardsView.FindIndex(item => item.Name == card.Prototype.Name);
                        if (index < 0) {
                            CardView cardView = new CardView(card) {
                                Amount = 0,
                                TextColor = "LightSteelBlue"
                            };
                            cardsView.Add(cardView);
                        }
                    }

                    cardsView = cardsView.OrderBy(o => o.Mana).ThenBy(o => o.Name).ToList();
                }
            }

            return cardsView;
        }

        private string GetDrawProbability(CardView cardView) {
            return (Math.Round(100 * (double) cardView.Amount / CardsInDeck, 1).ToString() + "%").PadLeft(6, ' ');
        }
    }
}
