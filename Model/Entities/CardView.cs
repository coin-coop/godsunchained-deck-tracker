using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GodsUnchained_Deck_Tracker.Model.Entities
{
    public class CardView
    {
        public CardView(Card card) {
            Name = card.Prototype.Name.PadRight(39, ' ').PadLeft(42, ' ');
            Mana = card.Prototype.Mana.ToString().PadLeft(2, ' ').PadRight(4, ' ');

            Amount = 1;
        }

        public string Name { get; set; }

        public string Mana { get; set; }

        public string Image { get; set; }

        public int Amount { get; set; }
    }
}
