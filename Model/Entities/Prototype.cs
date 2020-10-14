using GodsUnchained_Companion_App.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodsUnchained_Companion_App.Model.Entities
{
    public class Prototype
    {
        public Prototype(string name) {
            Name = name;
            Mana = 0;
        }

       public Prototype(string[] data) {
            Name = data[0];
            Id = Int32.Parse(data[1]);
            Effect = data[2];
            God = (God) Enum.Parse(typeof(God), data[3].First().ToString().ToUpper() + data[3].Substring(1));
            Rarity = (CardRarity) Enum.Parse(typeof(CardRarity), data[4].First().ToString().ToUpper() + data[4].Substring(1));
            Mana = Int32.Parse(data[5]);
            Type = (CardType) Enum.Parse(typeof(CardType), data[6].First().ToString().ToUpper() + data[6].Substring(1));
            Set = data[7];
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Effect { get; set; }

        public God God { get; set; }

        public CardRarity Rarity { get; set; }

        public int Mana { get; set; }

        public CardType Type { get; set; }

        public string Set { get; set; }
    }
}
