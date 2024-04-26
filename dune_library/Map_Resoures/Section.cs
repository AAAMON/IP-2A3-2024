using dune_library.Player_Resources;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resources {
  internal class Section(int sector, Territory territory, IDictionary<Faction, ISet<Section>> map_presences) {
    public int Origin_Sector => sector;

    public Territory Origin_Territory => territory;

    #region Storm

    public void Affect_By_Storm() {
      Local_Player_Presence.Clear();
      Delete_Spice();
    }

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

    #region Player Presences

    public IDictionary<Faction, (uint normal, uint special)> Local_Player_Presence { get; } = new Dictionary<Faction, (uint, uint)>();

    public IDictionary<Faction, ISet<Section>> Map_Presences => map_presences;

    public uint GetNormal(Faction faction) => Local_Player_Presence[faction].normal;

    public uint GetSpecial(Faction faction) => Local_Player_Presence[faction].special;

    /// <summary>
    /// <para>
    /// Adds a certain number of normal and special(Fedaykin, Sardaukar, Advisors) forces to this faction's presence;
    /// </para>
    /// <para>
    /// Will add the section to the faction's set of sections if the faction didn't have any forces in the section before adding them.
    /// </para>
    /// </summary>
    /// <param name="faction">The faction that will be affected by the changes</param>
    /// <param name="normal">The amount of normal forces to be added to this faction's presence</param>
    /// <param name="special">The amount of special forces to be added to this faction's presence</param>
    public void Add(Faction faction, uint normal, uint special) {
      if (Local_Player_Presence.TryGetValue(faction, out (uint normal, uint special) value)) {
        Local_Player_Presence[faction] = (value.normal + normal, value.special + special);
      } else {
        Local_Player_Presence[faction] = (normal, special);
        Map_Presences[faction].Add(this);
      }
    }

    /// <summary>
    /// <para>
    /// Converts the normal forces in this player's presence to special forces.
    /// </para>
    /// <para>
    /// this is mainly used by Bene Gesserit players.
    /// </para>
    /// </summary>
    /// <param name="faction"></param>
    public void FlipNormalToSpecial(Faction faction) {
      if (Local_Player_Presence.TryGetValue(faction, out (uint normal, uint special) value)) {
        Local_Player_Presence[faction] = (0, value.special + value.normal);
      }
    }

    /// <summary>
    /// <para>
    /// Converts the special forces in this player's presence to normal forces.
    /// </para>
    /// <para>
    /// this is mainly used by Bene Gesserit players.
    /// </para>
    /// </summary>
    /// <param name="faction"></param>
    public void FlipSpecialToNormal(Faction faction) {
      if (Local_Player_Presence.TryGetValue(faction, out (uint normal, uint special) value)) {
        Local_Player_Presence[faction] = (value.normal + value.special, 0);
      }
    }

    /// <summary>
    /// <para>
    /// Swaps the special and normal forces in this player's presence.
    /// </para>
    /// <para>
    /// this is mainly used by Bene Gesserit players.
    /// </para>
    /// </summary>
    /// <param name="faction"></param>
    public void Swap(Faction faction) {
      if (Local_Player_Presence.TryGetValue(faction, out (uint normal, uint special) value)) {
        Local_Player_Presence[faction] = (value.special, value.normal);
      }
    }

    /// <summary>
    /// <para>
    /// Removes a certain number of normal and special(Fedaykin, Sardaukar, Advisors) forces from a faction's presence;
    /// <para>
    /// <listheader>
    /// If all of this faction's forces have been removed:
    /// </listheader>
    /// <list type="number">
    /// <item>
    /// the faction's presence is removed from the section
    /// </item>
    /// <item>
    /// the section is also removed from that player's set of sections
    /// </item>
    /// </list>
    /// </para>
    /// </para>
    /// </summary>
    /// <param name="faction">The faction that will suffer the changes</param>
    /// <param name="normal">The amount of normal forces to be removed from this faction's presence</param>
    /// <param name="special">The amount of special forces to be removed from this faction's presence</param>
    /// <returns>
    /// True if the normal/special forces to be removed have been removed.
    /// This implies that the faction has a presence in the section;
    /// Otherwise, False.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// the number of normal or special forces exceeds the number of that faction's normal/special forces present
    /// </exception>
    public bool Remove(Faction faction, uint normal, uint special) {
      if (Local_Player_Presence.TryGetValue(faction, out (uint normal, uint special) value)) {
        if (value.normal < normal) {
          throw new ArgumentException("Tried to remove " + normal + " normal forces, while there are " + value.normal + " normal forces present", nameof(normal));
        }
        if (value.special < special) {
          throw new ArgumentException("Tried to remove " + special + " special forces, while there are " + value.special + " special forces present", nameof(special));
        }
        Local_Player_Presence[faction] = (value.normal - normal, value.special - special);
        if (Local_Player_Presence[faction] == (0, 0)) {
          Local_Player_Presence.Remove(faction);
          Map_Presences[faction].Remove(this);
        }
        return true;
      }
      return false;
    }

    /// <summary>
    /// <para>
    /// Fully removes this faction's presence;
    /// </para>
    /// <para>
    /// Will also remove this section from this faction's set of sections.
    /// </para>
    /// </summary>
    /// <param name="faction">The faction whose presence is to be removed</param>
    /// <returns></returns>
    public bool Remove(Faction faction) {
      if (Local_Player_Presence.ContainsKey(faction)) {
        Local_Player_Presence.Remove(faction);
        Map_Presences[faction].Remove(this);
        return true;
      }
      return false;
    }

    public int Number_Of_Factions_Present => Local_Player_Presence.Count;

    public bool Is_Present(Faction faction) => Local_Player_Presence.ContainsKey(faction);

    public bool Is_Full_Stronghold => Origin_Territory is Strongholds && Number_Of_Factions_Present == 2;

    #endregion
  }
}
