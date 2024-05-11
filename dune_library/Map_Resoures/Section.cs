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
using System.Text.Json.Serialization;

namespace dune_library.Map_Resources {
  public class Section {
    public Section(uint sector, Territory territory, uint id) {
      Origin_Sector = sector;
      Origin_Territory = territory;
      /*Origin_Territory_Id = territory.Id;*/
      Id = id;
    }

    public uint Id { get; }

    public uint Origin_Sector { get; }

    [JsonIgnore] public Territory Origin_Territory { get; }
    /*[JsonInclude] private int Origin_Territory_Id;*/ /* !!! CAN EASILY BE DEDUCED !!! */

    #region Storm

    public virtual void Delete_Spice() {}

    #endregion

    #region Section Linking

    private bool can_add_neighbors = true;

    [JsonIgnore] public ISet<Section> Neighboring_Sections { get; } = new System.Collections.Generic.HashSet<Section>();
    [JsonInclude] private IEnumerable<uint> Neighboring_Sections_Ids = [];

    public void Block_Adding_Neighbors() {
      can_add_neighbors = false;
      Neighboring_Sections_Ids = Neighboring_Sections.Select(section => section.Id);
    }

    public void Add_Neighbor(Section other) {
      if (can_add_neighbors == false) { return; }
      Neighboring_Sections.Add(other);
    }

    #endregion

    #region Forces

    [JsonPropertyOrder(999)] public Forces Forces { get; private set; } = new();

    [JsonIgnore] public bool Is_Full_Strongholds => Origin_Territory is Strongholds && Forces.Number_Of_Factions_Present >= 2;

    #endregion
  }
}
