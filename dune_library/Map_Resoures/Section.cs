using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resources {
  internal class Section {
    public Section(int sector, Territory region, int spice_capacity) {
      Origin_Sector = sector;
      Origin_Region = region;
      Spice_Capacity = spice_capacity;
    }

    public Section(int sector, Territory region) {
      Origin_Sector = sector;
      Origin_Region = region;
      Spice_Capacity = null;
    }

    public int Origin_Sector { get; }
    public Territory Origin_Region { get; }

    public int? Spice_Capacity { get; }

    public int Spice_Avaliable { get; private set; } = 0;

    public void Affect_By_Storm() {
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
