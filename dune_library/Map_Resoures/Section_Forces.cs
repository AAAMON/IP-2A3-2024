using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dune_library.Map_Resoures.Force_Containers;
using dune_library.Utils;
using dune_library.Player_Resources;
using System.Text.Json.Serialization;
using dune_library.Player_Resources.Factions;

namespace dune_library.Map_Resoures {
  public class Section_Forces {
    public Section_Forces() {
      Atreides_Forces = new(0);
      Bene_Gesserit_Forces = new(0);
      Emperor_Forces = new(0, 0);
      Fremen_Forces = new(0, 0);
      Spacing_Guild_Forces = new(0);
      Harkonnen_Forces = new(0);
      Emptiness_Checkables = Emptiness_Checkables_Default;
    }

    [JsonConstructor]
    public Section_Forces(
      Atreides.Forces atreides_forces,
      Bene_Gesserit.Forces bene_gesserit_forces,
      Emperor.Forces emperor_forces,
      Fremen.Forces fremen_forces,
      Spacing_Guild.Forces spacing_guild_forces,
      Harkonnen.Forces harkonnen_forces
    ) {
      Atreides_Forces = atreides_forces;
      Bene_Gesserit_Forces = bene_gesserit_forces;
      Emperor_Forces = emperor_forces;
      Fremen_Forces = fremen_forces;
      Spacing_Guild_Forces = spacing_guild_forces;
      Harkonnen_Forces = harkonnen_forces;
      Emptiness_Checkables = Emptiness_Checkables_Default;
    }
    [JsonInclude] private Atreides.Forces Atreides_Forces { get; }
    [JsonInclude] private Bene_Gesserit.Forces Bene_Gesserit_Forces { get; }
    [JsonInclude] private Emperor.Forces Emperor_Forces { get; }
    [JsonInclude] private Fremen.Forces Fremen_Forces { get; }
    [JsonInclude] private Spacing_Guild.Forces Spacing_Guild_Forces { get; }
    [JsonInclude] private Harkonnen.Forces Harkonnen_Forces { get; }

    private IReadOnlyDictionary<Faction_Class, I_Emptiness_Checkable> Emptiness_Checkables;
    private IReadOnlyDictionary<Faction_Class, I_Emptiness_Checkable> Emptiness_Checkables_Default =>
      new Dictionary<Faction_Class, I_Emptiness_Checkable> {
      [Atreides.Instance] = Atreides_Forces,
      [Bene_Gesserit.Instance] = Bene_Gesserit_Forces,
      [Emperor.Instance] = Emperor_Forces,
      [Fremen.Instance] = Fremen_Forces,
      [Spacing_Guild.Instance] = Spacing_Guild_Forces,
      [Harkonnen.Instance] = Harkonnen_Forces,
    };

    [JsonIgnore]
    public uint Number_Of_Factions_Present =>
      (uint)Emptiness_Checkables.Values.Select(forces => forces.Is_Empty.To_Int()).Sum();

    [JsonIgnore]
    public bool No_Faction_Is_Present => Number_Of_Factions_Present == 0;

    public void Transfer_To(Faction_Class faction, Section_Forces destination, uint to_transfer) {
      ((Action)(faction switch {
        Atreides => () => Atreides_Forces.Transfer_To(destination.Atreides_Forces, to_transfer),
        Bene_Gesserit => () => Bene_Gesserit_Forces.Transfer_To(destination.Bene_Gesserit_Forces, to_transfer),
        Emperor => () => Emperor_Forces.Transfer_To(destination.Emperor_Forces, to_transfer, 0),
        Fremen => () => Fremen_Forces.Transfer_To(destination.Fremen_Forces, to_transfer, 0),
        Spacing_Guild => () => Spacing_Guild_Forces.Transfer_To(destination.Spacing_Guild_Forces, to_transfer),
        Harkonnen => () => Harkonnen_Forces.Transfer_To(destination.Harkonnen_Forces, to_transfer),
      })).Invoke();
    }

    public void Transfer_To(Faction_Class faction, Section_Forces destination, uint to_transfer_normal, uint to_transfer_special) {
      ((Action)(faction switch {
        Emperor => () => Emperor_Forces.Transfer_To(destination.Emperor_Forces, to_transfer_normal, to_transfer_special),
        Fremen => () => Fremen_Forces.Transfer_To(destination.Fremen_Forces, to_transfer_normal, to_transfer_special),
        _ => () => throw new ArgumentException("Faction " + faction.GetType().Name + " does not have special forces"),
      })).Invoke();
    }

    public void Remove_By_Storm(Section_Forces graveyard) {
      Atreides_Forces.Remove_By_Storm(graveyard.Atreides_Forces);
      Bene_Gesserit_Forces.Remove_By_Storm(graveyard.Bene_Gesserit_Forces);
      Emperor_Forces.Remove_By_Storm(graveyard.Emperor_Forces);
      Fremen_Forces.Remove_By_Storm(graveyard.Fremen_Forces);
      Spacing_Guild_Forces.Remove_By_Storm(graveyard.Spacing_Guild_Forces);
      Harkonnen_Forces.Remove_By_Storm(graveyard.Harkonnen_Forces);
    }

    public bool Is_Present(Faction_Class faction) => !Emptiness_Checkables[faction].Is_Empty;
  }
}
