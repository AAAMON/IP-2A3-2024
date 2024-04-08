using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map {
  internal class Strongholds : Region {
    public override List<Section> Sections { get; }
    public Strongholds(string name, ushort sector) : base(name) {
      Sections = [new Section(Map.To_Sector(sector), this)];
    }
  }
}
