using GodsUnchained_Companion_App.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GodsUnchained_Companion_App.Model.Entities
{
    public class Spell : Prototype
    {
        public Spell(string name) : base(name) {
            Name = name;
            Type = CardType.Spell;
        }
    }
}
