using dune_library.Player_Resources;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resources {
  public class Rock : Territory {
    public override IReadOnlyList<Section> Sections { get; }
    public Rock(string name, uint first_sector, uint sectors_spanned, uint id, ref uint section_counter) : base(name, id) {
      uint counter = section_counter;
      Sections = Extensions.Range(first_sector, sectors_spanned)
                           .Select(sector => new Section(sector.To_Sector(), this, counter++))
                           .ToList();
      section_counter = counter;
    }
  }
}
