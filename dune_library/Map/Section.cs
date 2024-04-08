using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map {
  internal class Section {
    public Section(ushort sector, Region region, ushort spice_capacity) {
      Origin_Sector = sector;
      Origin_Region = region;
      Spice_Capacity = spice_capacity;
    }

    public Section(ushort sector, Region region) {
      Origin_Sector = sector;
      Origin_Region = region;
      Spice_Capacity = null;
    }

    public ushort Origin_Sector { get; }
    public Region Origin_Region { get; }

    public ushort? Spice_Capacity { get; }

    public ushort Spice_Avaliable { get; private set; } = 0;

    public void affect_by_storm() {
      // will have to remove enemy presence here
      Spice_Avaliable = 0;
    }

    private bool can_add_neighbors = true;
    public HashSet<Section> Neighboring_Sections { get; } = [];

    public void Block_Adding_Neighbors() {
      can_add_neighbors = false;
    }

    public void Add_Neighbor(Section other) {
      if (can_add_neighbors == false) { return; }
      Neighboring_Sections.Add(other);
    }
  }
}
