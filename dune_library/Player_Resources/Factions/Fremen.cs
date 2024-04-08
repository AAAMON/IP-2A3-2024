using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources.Factions
{
    internal class Fremen : Faction, I_Special_Win_Conditions
    {
        public Fremen(string name) : base(name)
        {
        }

        public bool CheckWin(int turnCount, Faction faction)
        {
            //verifica
            return false;
        }
    }
}
