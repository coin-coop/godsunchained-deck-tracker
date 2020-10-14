using GodsUnchained_Companion_App.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GodsUnchained_Companion_App.Model.Enums
{
    public enum CardMarker
    {
        [AssetName(null)]
        None = ' ',

        [AssetName("/Images/card-icon-mulligan.png")]
        Mulliganed = 'M',

        [AssetName("/Images/card-icon-keep.png")]
        Kept = 'K',

        [AssetName("/Images/card-icon-created.png")]
        Created = 'C',

        [AssetName("/Images/card-icon-returned.png")]
        Returned = 'R',

        [AssetName("/Images/card-icon-stolen.png")]
        Stolen = 'S'
    }
}
