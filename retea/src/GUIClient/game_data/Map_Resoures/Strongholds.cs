using dune_library.Player_Resources;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resources {
  public class Strongholds : Territory {
    public override IReadOnlyList<Section> Sections { get; }
    public Strongholds(string name, uint sector, uint id, ref uint section_counter) : base(name, id) {
      Sections = [new Section(sector.To_Sector(), this, section_counter++)];
    }
    public static implicit operator Section(Strongholds obj) => obj.Sections[0];
  }
}
