using dune_library.Player_Resources;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils
{
    public class Game_Winners
    {
        public Option<Faction> faction1 { get; set; }
        public Option<Faction> faction2 { get; set; }
        public Game_Winners(Faction faction)
        {
            faction1 = faction;
        }
        public Game_Winners(Faction faction1, Faction faction2)
        {
            this.faction1 = faction1;
            this.faction2 = faction2;
        }
        public Game_Winners()
        {

        }
        public bool hasWinner()
        {
            if(faction1.IsSome || faction2.IsSome)
            {
                return true;
            }
            return false;
        }

    }
}
