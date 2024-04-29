using dune_library.Player_Resources;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resources {
  public class Section(int sector, Territory territory) {
    private static int counter = 0;

    public int Id { get; } = counter++;

    public int Origin_Sector => sector;

    public Territory Origin_Territory => territory;

    #region Storm

    public virtual void Delete_Spice() { }

    #endregion

    #region Section Linking

    private bool can_add_neighbors = true;
    public ISet<Section> Neighboring_Sections { get; } = new HashSet<Section>();

    public void Block_Adding_Neighbors() {
      can_add_neighbors = false;
    }

    public void Add_Neighbor(Section other) {
      if (can_add_neighbors == false) { return; }
      Neighboring_Sections.Add(other);
    }

    #endregion
  }
}
