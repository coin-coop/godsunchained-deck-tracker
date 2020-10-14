using GodsUnchained_Companion_App.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsUnchained_Companion_App.Model.Entities
{
    public class Deck {

        // TODO: add handling for null
        public Deck(string name, string god, List<Card> cards, int games) {
            Name = name;
            God = (God) Enum.Parse(typeof(God), god.First().ToString().ToUpper() + god.Substring(1));
            Cards = cards;
            Games = games;
        }

        // TODO: extend with god power

        public string Name { get; set; }

        public God God { get; set; }

        public List<Card> Cards { get; set; }

        public int Games { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }
    }
}
