using dune_library.Player_Resources;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils
{
    public class Highest_Bid
    {
        public Option<Faction> faction;
        public uint bid;
        public Highest_Bid(Faction faction, uint bid) {
            this.faction = faction;
            this.bid = bid;
        }
        public Highest_Bid() { }
    }
}
