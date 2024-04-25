using dune_library.Player_Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resources {
  internal class Rock : Territory {
    public override IReadOnlyList<Section> Sections { get; }
    public Rock(string name, int first_sector, int sectors_spanned, IDictionary<Faction, ISet<Section>> presences) : base(name) {
      Sections = Enumerable.Range(first_sector, sectors_spanned)
                           .Select(sector => new Section(Map.To_Sector(sector), this, presences))
                           .ToList();
    }
  }
}
