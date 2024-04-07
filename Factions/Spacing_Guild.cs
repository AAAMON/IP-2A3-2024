using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player.Factions
{
    internal class Spacing_Guild : Faction, I_Special_Win_Conditions
    {
        public Spacing_Guild(string name) : base(name)
        {
        }

        public bool CheckWin(int turnCount, Faction faction)
        {
            //verifica
            return false;
        }
    }
}
