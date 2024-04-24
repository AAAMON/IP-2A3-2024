using dune_library.Map_Resources;
using dune_library.Spice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  internal class Perspective {
    public Faction Faction { get; set; }

    public Map Map { get; set; }

    public int Storm_Sector => Map.Storm_Sector;

    public IDictionary<Faction, Player_Info> Player_Infos { get; set; }

    public Own_Player_Info Own_Player_Info => (Own_Player_Info)Player_Infos[Faction];

    public int Round { get; set; }

    public int Phase { get; set; }

    public int Moment { get; set; }

    public Territory_Card Last_Spice_Card { get; set; }

  }
}
