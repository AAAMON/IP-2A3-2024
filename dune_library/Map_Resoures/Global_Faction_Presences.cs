using dune_library.Map_Resources;
using dune_library.Player_Resources;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Map_Resoures {
  public class Global_Faction_Presences {
    public Global_Faction_Presences() {
      Presences = new Dictionary<Section, Local_Faction_Presences>();

      Presences_By_Faction = new ReadOnlyDictionary<Faction, ISet<Section>>(
        new Dictionary<Faction, ISet<Section>>(
          Enum.GetValues<Faction>().Select(faction =>
            new KeyValuePair<Faction, ISet<Section>>(faction, new HashSet<Section>())
          )
        )
      );
    }

    [JsonConstructor]
    public Global_Faction_Presences(
      IDictionary<Section, Local_Faction_Presences> presences,
      IReadOnlyDictionary<Faction, ISet<Section>> presences_by_faction
    ) {
      Presences = presences;
      Presences_By_Faction = presences_by_faction;
    }

    [JsonInclude]
    private IDictionary<Section, Local_Faction_Presences> Presences { get; }

    [JsonInclude]
    private IReadOnlyDictionary<Faction, ISet<Section>> Presences_By_Faction { get; }

    public ISet<Section> Of(Faction faction) => Presences_By_Faction[faction];

    public bool Has_Presences(Section section) => Presences.ContainsKey(section);

    public Forces_Container From_Of(Section section, Faction faction) {
      Throw_If_Does_Not_Have_Presences(section);
      return Presences[section].Of(faction);
    }

    public bool Is_Present(Section section, Faction faction) {
      Throw_If_Does_Not_Have_Presences(section);
      return Presences[section].Is_Present(faction);
    }

    private void Throw_If_Does_Not_Have_Presences(Section section) {
      if (!Has_Presences(section)) {
        throw new ArgumentException("This section (" + section + ") does not have any presences", nameof(section));
      }
    }

    public int Number_Of_Factions_Present(Section section) {
      if (!Presences.ContainsKey(section)) {
        return 0;
      }
      return Presences[section].Number_Of_Factions_Present;
    }

    public bool Is_Full_Strongholds(Section section) => section.Origin_Territory is Strongholds && Number_Of_Factions_Present(section) == 2;

    private bool Is_Empty(Section section) => Number_Of_Factions_Present(section) == 0;

    #region Add, Bene Gesserit swaps and Remove

    /// <summary>
    /// <para>
    /// Adds a certain number of normal and special(Fedaykin, Sardaukar, Advisors) forces to this faction's presence in this section;
    /// </para>
    /// <para>
    /// Will add the section to the faction's set of sections if the faction didn't have any forces in the section before adding them.
    /// </para>
    /// </summary>
    /// <param name="section">The section that will be affected by the changes</param>
    /// <param name="faction">The faction that will be affected by the changes</param>
    /// <param name="normal">The amount of normal forces to be added to this faction's presence</param>
    /// <param name="special">The amount of special forces to be added to this faction's presence</param>
    public void Add(Section section, Faction faction, uint normal, uint special) {
      if (!Has_Presences(section)) {
        Presences[section] = new();
      }
      Presences[section].Add(faction, normal, special);
      Presences_By_Faction[faction].Add(section);
    }

    public void Add(Section section, Faction faction, Forces_Container other) => Add(section, faction, other.Normal, other.Special);

    public void Flip_Normal_To_Special(Section section, Faction faction) {
      Throw_If_Does_Not_Have_Presences(section);
      Presences[section].Flip_Normal_To_Special(faction);
    }

    public void Flip_Special_To_Normal(Section section, Faction faction) {
      Throw_If_Does_Not_Have_Presences(section);
      Presences[section].Flip_Special_To_Normal(faction);
    }

    public void Swap(Section section, Faction faction) {
      Throw_If_Does_Not_Have_Presences(section);
      Presences[section].Swap(faction);
    }

    /// <summary>
    /// <para>
    /// Removes a certain number of normal and special(Fedaykin, Sardaukar, Advisors) forces from this faction's presence in this section;
    /// </para>
    /// <para>
    /// <listheader>
    /// If all of this faction's forces have been removed:
    /// </listheader>
    /// <list type="number">
    /// <item>
    /// the faction's presence is removed from the section
    /// </item>
    /// <item>
    /// the section is also removed from that faction's set of sections
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// If all of the presences from this section are removed, the section is also removed from the global presences dictionary
    /// </para>
    /// </summary>
    /// <param name="section">The section that will be affected by the changes</param>
    /// <param name="faction">The faction that will suffer the changes</param>
    /// <param name="normal">The amount of normal forces to be removed from this faction's presence</param>
    /// <param name="special">The amount of special forces to be removed from this faction's presence</param>
    /// <returns>
    /// True if the normal/special forces to be removed have been removed.
    /// This implies that the faction has a presence in the section;
    /// Otherwise, False.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// the number of normal or special forces exceeds the number of thais faction's normal/special forces present,
    /// this section has no presences, or this faction has no force in this section
    /// </exception>
    public void Remove(Section section, Faction faction, uint normal, uint special) {
      Throw_If_Does_Not_Have_Presences(section);
      Presences[section].Remove(faction, normal, special);
      if (!Presences[section].Is_Present(faction)) {
        Presences_By_Faction[faction].Remove(section);
      }
      if (Is_Empty(section)) {
        Presences.Remove(section);
      }
    }

    public void Remove(Section section, Faction faction, Forces_Container other) => Remove(section, faction, other.Normal, other.Special);

    /// <summary>
    /// Removes half of this faction's forces from this section
    /// </summary>
    /// <param name="section">The section that will be affected by the changes</param>
    /// <param name="faction">The faction whose presence is to be halved</param>
    public void Remove_Half(Section section, Faction faction) {
      Throw_If_Does_Not_Have_Presences(section);
      (uint n, uint s) = Presences[section].Of(faction);
      Remove(section, faction, n - n / 2, s - s / 2);
    }

    /// <summary>
    /// <para>
    /// Fully removes this faction's presence from this section;
    /// </para>
    /// </summary>
    /// <param name="section">The section that will be affected by the changes</param>
    /// <param name="faction">The faction whose presence is to be removed</param>
    /// <returns></returns>
    public void Remove(Section section, Faction faction) {
      Throw_If_Does_Not_Have_Presences(section);
      (uint n, uint s) = Presences[section].Of(faction);
      Remove(section, faction, n, s);
    }

    /// <summary>
    /// Fully removes all presences from this section;
    /// </summary>
    /// <param name="section">The section that will be affected by the changes</param>
    public void Remove(Section section) {
      Throw_If_Does_Not_Have_Presences(section);
      Enum.GetValues<Faction>().Filter(faction => Presences[section].Is_Present(faction)).ForEach(faction => Remove(section, faction));
    }

    /// <summary>
    /// Fully removes all presences from this section, except half of the Fremen forces;
    /// </summary>
    /// <param name="section">The section that will be affected by the changes</param>
    public void Remove_But_Keep_Half_Of_Fremen(Section section) {
      Throw_If_Does_Not_Have_Presences(section);
      Enum.GetValues<Faction>().Filter(faction => Presences[section].Is_Present(faction)).ForEach(faction => {
        ((Action)(faction switch {
          Faction.Fremen => () => Remove_Half(section, faction),
          _ => () => Remove(section, faction),
        })).Invoke();
      });
    }

    #endregion
  }
}
