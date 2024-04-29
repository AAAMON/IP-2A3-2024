using dune_library.Player_Resources;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resources {
  public class Rock : Territory {
    public override IReadOnlyList<Section> Sections { get; }
    public Rock(string name, int first_sector, int sectors_spanned) : base(name) {
      Sections = Enumerable.Range(first_sector, sectors_spanned)
                           .Select(sector => new Section(sector.To_Sector(), this))
                           .ToList();
    }
  }
}
