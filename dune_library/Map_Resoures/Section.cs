using dune_library.Player_Resources;
using dune_library.Utils;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dune_library.Map_Resoures;

namespace dune_library.Map_Resources {
  public class Section(int sector, Territory territory) {
    private static int counter = 0;

    public int Id { get; } = counter++;

    public int Origin_Sector { get; } = sector;

    public Territory Origin_Territory { get; } = territory;

    #region presences

    public Section_Forces Forces { get; private set; } = new();

    public void Copy_Forces_From(Section_Forces forces) {
      Forces = forces;
    }

    public bool Is_Full_Strongholds => Origin_Territory is Strongholds && Forces.Number_Of_Factions_Present >= 2;

    #endregion

    #region Storm

    public virtual void Delete_Spice() {}

    #endregion

    #region Section Linking

    private bool can_add_neighbors = true;
    public ISet<Section> Neighboring_Sections { get; } = new System.Collections.Generic.HashSet<Section>();

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
