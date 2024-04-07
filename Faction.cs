using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dune_library.Treachery_Cards;

namespace dune_library.Player
{
    internal class Faction
    {
        private int Troops { get; set; }
        private string Name { get; set; }
        private List<Treachery_Card> TreacheryCards { get; set; }
        public List<General> Generals { get; set; }
        public Graveyard Graveyard { get; set; }
        public Dictionary<dune_library.Map.Sector, int> TroopPositions { get; set; }

        public Faction(string name)
        {
            Name = name;
            Troops = 0;
            Graveyard = new Graveyard();
            TroopPositions = new Dictionary<dune_library.Map.Sector, int>();
        }
    }
}
