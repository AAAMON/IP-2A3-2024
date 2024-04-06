using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map {
  internal class Section {
    public Section(ushort sector, ushort spice_capacity) {
      Origin_Sector = sector;
      Spice_Capacity = spice_capacity;
      Spice_Avaliable = 0;
    }

    public Section(ushort sector) {
      Origin_Sector = sector;
      Spice_Capacity = null;
      Spice_Avaliable = 0;
    }

    public ushort Origin_Sector { get; }

    public ushort? Spice_Capacity { get; } 

    public ushort Spice_Avaliable { get; private set; }

    public void affect_by_storm() {
      // will have to remove enemy presence here
      Spice_Avaliable = 0;
    }
  }
}
