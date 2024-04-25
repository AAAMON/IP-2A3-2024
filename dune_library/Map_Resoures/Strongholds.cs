using dune_library.Player_Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resources {
  internal class Strongholds : Territory {
    public override List<Section> Sections { get; }
    public Strongholds(string name, ushort sector, IDictionary<Faction, ISet<Section>> presences) : base(name) {
      Sections = [new Section(Map.To_Sector(sector), this, presences)];
    }
  }
}
