using GodsUnchained_Deck_Tracker.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GodsUnchained_Deck_Tracker.Tracker
{
    interface IDeckTracker
    {
        List<CardView> GetCardsView(List<Card> cards, Deck selectedDeck = null, bool currentCards = false);

    }
}
