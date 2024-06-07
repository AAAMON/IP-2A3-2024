using dune_library.Map_Resources;
using dune_library.Map_Resoures;
using dune_library.Player_Resources;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils
{
    public class Faction_Battles
    {
        public Option<Faction> faction { get; set; }
        public Option<Faction> enemy { get; set; }
        public List<uint> Battle_Sections { get; set; }
        public uint Chosen_Battle_Section { get; set; }

        public Faction_Battles() {
            Battle_Sections = new List<uint>();
            Chosen_Battle_Section = 0;
        }
    }
}
