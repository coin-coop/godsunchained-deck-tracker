using System;
using System.Collections.Generic;
using System.Text;

namespace GodsUnchained_Companion_App.Utilities.Attributes
{
    class AssetNameAttribute : Attribute
    {
        public string Name { get; }

        public AssetNameAttribute(string name) {
            Name = name;
        }
    }
}
