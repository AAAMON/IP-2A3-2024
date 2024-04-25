using dune_library.Player_Resources;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resources {
  internal class Section(int sector, Territory territory, IDictionary<Faction, ISet<Section>> presences) {
    public int Origin_Sector => sector;

    public Territory Origin_Territory => territory;

    #region Storm

    public void Affect_By_Storm() {
      Player_Presence.Clear();
      Remove_Spice();
    }

    public virtual void Remove_Spice() { }

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

    #region Player Presences

    public IDictionary<Faction, (uint normal, uint special)> Player_Presence { get; } = new Dictionary<Faction, (uint, uint)>();

    private IDictionary<Faction, ISet<Section>> Presences { get; } = presences;

    public void AddBoth(Faction faction, uint normal, uint special) {
      if (Player_Presence.TryGetValue(faction, out (uint normal, uint special) value)) {
        Player_Presence[faction] = (value.normal + normal, value.special + special);
      } else {
        Player_Presence[faction] = (normal, special);
        Presences[faction].Add(this);
      }
    }

    public void AddNormal(Faction faction, uint value) => AddBoth(faction, value, 0);

    public void AddSpecial(Faction faction, uint value) => AddBoth(faction, 0, value);

    public void FlipNormalToSpecial(Faction faction) {
      if (Player_Presence.TryGetValue(faction, out (uint normal, uint special) value)) {
        Player_Presence[faction] = (0, value.special + value.normal);
      }
    }

    public void FlipSpecialToNormal(Faction faction) {
      if (Player_Presence.TryGetValue(faction, out (uint normal, uint special) value)) {
        Player_Presence[faction] = (value.normal + value.special, 0);
      }
    }

    public void Swap(Faction faction) {
      if (Player_Presence.TryGetValue(faction, out (uint normal, uint special) value)) {
        Player_Presence[faction] = (value.special, value.normal);
      }
    }

    public bool RemoveBoth(Faction faction, uint normal, uint special) {
      if (Player_Presence.TryGetValue(faction, out (uint normal, uint special) value)) {
        if (value.normal < normal) {
          throw new ArgumentException("Tried to remove " + normal + " normal forces, while there are " + value.normal + " normal forces present", nameof(normal));
        }
        if (value.special < special) {
          throw new ArgumentException("Tried to remove " + special + " special forces, while there are " + value.special + " special forces present", nameof(special));
        }
        Player_Presence[faction] = (value.normal - normal, value.special - special);
        if (Player_Presence[faction] == (0, 0)) {
          Player_Presence.Remove(faction); // always true
          Presences[faction].Remove(this);
        }
        return true;
      }
      return false;
    }

    public bool RemoveNormal(Faction faction, uint value) => RemoveBoth(faction, value, 0);

    public bool RemoveSpecial(Faction faction, uint value) => RemoveBoth(faction, 0, value);

    #endregion
  }
}
