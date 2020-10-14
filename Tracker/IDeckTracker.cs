using GodsUnchained_Companion_App.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GodsUnchained_Companion_App.Tracker
{
    interface IDeckTracker
    {
        List<CardView> GetCardsView(List<Card> cards, Deck selectedDeck = null, bool currentCards = false);

    }
}
