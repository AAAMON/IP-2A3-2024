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
using Extensions = dune_library.Utils.Extensions;

namespace dune_library.Map_Resources {
  public class Sand : Territory {
    public override IReadOnlyList<Section> Sections { get; }

    public Sand(string name, uint first_sector, List<Option<uint>> spice_capacities, uint id, ref uint section_counter) : base(name, id) {
      uint current_sector = first_sector;
      uint counter = section_counter;
      Sections = spice_capacities.Select(spice_capacity =>
                                    spice_capacity.Match(
                                      value => new With_Spice(current_sector++.To_Sector(), this, counter++, value),
                                      () => new Section(current_sector++.To_Sector(), this, counter++)
                                    )
                                  ).ToList();
      section_counter = counter;
    }

    public Sand(string name, uint first_sector, uint sectors_spanned, uint id, ref uint section_counter) : base(name, id) {
      uint counter = section_counter;
      Sections = Extensions.Range(first_sector, sectors_spanned).Select(sector => new Section(sector.To_Sector(), this, counter++)).ToList();
      section_counter = counter;
    }
  }
}
