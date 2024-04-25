using dune_library.Map_Resoures;
using dune_library.Player_Resources;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace dune_library.Map_Resources {
  internal class Sand : Territory {
    public override IReadOnlyList<Section> Sections { get; }

    public Sand(string name, int first_sector, List<Option<int>> spice_capacities, IDictionary<Faction, ISet<Section>> presences) : base(name) {
      int current_sector = first_sector;
      Sections = spice_capacities.Select(spice_capacity =>
                                    spice_capacity.Match(
                                      value => new With_Spice(Map.To_Sector(current_sector++), this, value),
                                      () => new Section(Map.To_Sector(current_sector++), this, presences)
                                    )
                                  ).ToList();
    }

    public Sand(string name, int first_sector, int sectors_spanned, IDictionary<Faction, ISet<Section>> presences) : base(name) {
      Sections = Enumerable.Range(first_sector, sectors_spanned).Select(sector => new Section(Map.To_Sector(sector), this, presences)).ToList();
    }
  }
}
