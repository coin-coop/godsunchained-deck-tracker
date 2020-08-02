using System;
using System.Collections.Generic;
using System.Text;

namespace GodsUnchained_Deck_Tracker.Utilities.Attributes
{
    class AssetNameAttribute : Attribute
    {
        public string Name { get; }

        public AssetNameAttribute(string name) {
            Name = name;
        }
    }
}
