using GodsUnchained_Companion_App.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GodsUnchained_Companion_App.Model.Entities
{
    public class Creature : Prototype {

        public Creature(string name) : base(name) {
            Name = name;
            Type = CardType.Creature;
        }

        public Tribe Tribe { get; set; }

        public int Attack { get; set; }

        public int Health { get; set; }
    }
}
