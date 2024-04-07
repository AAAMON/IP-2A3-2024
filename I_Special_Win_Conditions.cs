using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player
{
    internal interface I_Special_Win_Conditions
    {
        bool CheckWin(int turnCount, Faction faction);
    }
}
