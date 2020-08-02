using GodsUnchained_Deck_Tracker.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsUnchained_Deck_Tracker.Model.Entities
{
    public class Deck {

        // TODO: add handling for null
        public Deck(string name, string god, List<Card> cards) {
            Name = name;
            God = (God) Enum.Parse(typeof(God), god.First().ToString().ToUpper() + god.Substring(1));
            Cards = cards;
        }

        // TODO: extend with god power

        public string Name { get; set; }

        public God God { get; set; }

        public List<Card> Cards { get; set; }
    }
}
