using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GodsUnchained_Deck_Tracker.Model.Entities
{
    public class CardView
    {
        public CardView(Card card) {
            Name = card.Prototype.Name;
            Mana = card.Prototype.Mana.ToString().PadLeft(3, ' ').PadRight(4, ' ');

            Amount = 1;
            TextColor = "AliceBlue";
        }

        public string Name { get; set; }

        public string NameMain {
            get => Name.PadRight(39, ' ').PadLeft(42, ' ');
        }

        public string NameTracker {
            get => Name.PadRight(32, ' ').PadLeft(34, ' ');
        }

        public string NameTrackerExtra {
            get => Name.PadRight(48, ' ').PadLeft(50, ' ');
        }

        public string Mana { get; set; }

        public string Image { get; set; }

        public int Amount { get; set; }

        public string DrawProbability { get; set; }

        public string TextColor { get; set; }

        public string AmountText {
            get => "  " + Amount.ToString() + "  ";
        }
    }
}
