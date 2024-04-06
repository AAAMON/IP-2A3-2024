using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map {
  internal class Strongholds : Region {
    public Strongholds(string name, ushort sector) : base(name) {
      sections = [new Section(Map.To_Sector(sector))];
    }
  }
}
