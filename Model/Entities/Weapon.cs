using GodsUnchained_Deck_Tracker.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GodsUnchained_Deck_Tracker.Model.Entities
{
    public class Weapon : Prototype
    {
        public Weapon(string name) : base(name) {
            Name = name;
            Type = CardType.Weapon;
        }

        public int Attack { get; set; }

        public int Durability { get; set; }
    }
}
