using System;
using System.Collections.Generic;
using System.Text;

namespace GodsUnchained_Companion_App.Model.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public int TotalExp { get; set; }

        public int ExpToNextLevel { get; set; }

        public int Wins { get; set; }

        public int Loses { get; set; }

        public int Rating { get; set; }

        public string Rank { get; set; }

        public int WinPoints { get; set; }

        public int LossPoints { get; set; }
    }
}
