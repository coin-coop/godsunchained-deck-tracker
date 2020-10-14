using System;
using System.Collections.Generic;
using System.Text;

namespace GodsUnchained_Companion_App.Model.Entities
{
    public enum Rank
    {
        RustedBronze = 1,
        PurifiedBronze = 2,
        RustedIron = 3,
        PurifiedIron = 4,
        ImpactMeteorite = 5,
        AstralMeteorite = 6, 
        TwilightShadow = 7,
        MidnightShadow = 8,
        AuricGold = 9,
        SolarGold = 10,
        EtherealDiamond = 11, //string.Concat(val.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        Mythic = 12 //Enum.GetName(typeof(Rank), value);
    }
}
