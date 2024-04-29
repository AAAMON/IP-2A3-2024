using dune_library.Player_Resources;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resources {
  public class Strongholds : Territory {
    public override List<Section> Sections { get; }
    public Strongholds(string name, int sector) : base(name) {
      Sections = [new Section(sector.To_Sector(), this)];
    }
  }
}
