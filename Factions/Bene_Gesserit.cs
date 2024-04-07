using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player.Factions
{
    internal class Bene_Gesserit
    {
        public bool WinCondition { get; set; }
        public bool CheckWin(int turnCount, Faction faction)
        {
            return WinCondition;
        }
    }
}
