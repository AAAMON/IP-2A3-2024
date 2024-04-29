using dune_library.Map_Resoures;
using dune_library.Player_Resources;
using dune_library.Utils;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace dune_library.Map_Resources {
  public class Sand : Territory {
    public override IReadOnlyList<Section> Sections { get; }

    public Sand(string name, int first_sector, List<Option<int>> spice_capacities) : base(name) {
      int current_sector = first_sector;
      Sections = spice_capacities.Select(spice_capacity =>
                                    spice_capacity.Match(
                                      value => new With_Spice(current_sector++.To_Sector(), this, value),
                                      () => new Section(current_sector++.To_Sector(), this)
                                    )
                                  ).ToList();
    }

    public Sand(string name, int first_sector, int sectors_spanned) : base(name) {
      Sections = Enumerable.Range(first_sector, sectors_spanned).Select(sector => new Section(sector.To_Sector(), this)).ToList();
    }
  }
}
