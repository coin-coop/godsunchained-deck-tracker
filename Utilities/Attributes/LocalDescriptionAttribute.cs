using System;
using System.Collections.Generic;
using System.Text;

namespace GodsUnchained_Companion_App.Utilities.Attributes
{
    public class LocalDescriptionAttribute : Attribute
    {
        public string LocalDescription { get; }

        public LocalDescriptionAttribute(string key, bool upper = false) {
            LocalDescription = LocalUtil.Get(key, upper)?.Replace("\\n", Environment.NewLine);
        }
    }
}
