using dune_library.Treachery_Cards;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static dune_library.Utils.Exceptions;

namespace dune_library.Player_Resources {
  public class Knowledge_Manager {
    public Knowledge_Manager(IReadOnlySet<Faction> factions_in_play) {
      Faction_Knowledge_Dict = factions_in_play.Select(faction =>
        new KeyValuePair<Faction, I_Faction_Knowledge>(faction, new Faction_Knowledge(factions_in_play))
      ).ToDictionary();
    }

    private IDictionary<Faction, I_Faction_Knowledge> Faction_Knowledge_Dict { get; }

    public I_Faction_Knowledge_Read_Only Of(Faction faction) => Faction_Knowledge_Dict[faction];

    public bool Init_Traitors(Faction faction, IReadOnlyList<General> traitors, IReadOnlyList<General> discarded_traitors) {
      if (Faction_Knowledge_Dict.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      return Faction_Knowledge_Dict[faction].Init_Traitors(traitors, discarded_traitors);
    }

    public void Add_Spice(Faction faction, uint to_add) {
      if (Faction_Knowledge_Dict.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      Faction_Knowledge_Dict[faction].Add_Spice(to_add);
    }

    public bool Remove_Spice(Faction faction, uint to_remove) {
      if (Faction_Knowledge_Dict.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      return Faction_Knowledge_Dict[faction].Remove_Spice(to_remove);
    }

    //transfers and such

    public void Add_Treachery_Card(Faction faction, Treachery_Card to_add) {
      if (Faction_Knowledge_Dict.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      Faction_Knowledge_Dict[faction].Add_Treachery_Card(to_add);
      Faction_Knowledge_Dict.Keys.ForEach(to_update =>
        Faction_Knowledge_Dict[to_update].Add_To_Number_Of_Treachery_Cards_Of(faction)
      );
    }

    public bool Remove_Treachery_Card(Faction faction, Treachery_Card to_remove) {
      if (Faction_Knowledge_Dict.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      if (Faction_Knowledge_Dict.Keys.Any(to_update =>
        Faction_Knowledge_Dict[to_update].Number_Of_Treachery_Cards_Is_Not_Zero(faction) == false
      )) {
        return false;
      }
      if (Faction_Knowledge_Dict[faction].Remove_Treachery_Card(to_remove) == false) {
        return false;
      }
      Faction_Knowledge_Dict.Keys.ForEach(to_update =>
        Faction_Knowledge_Dict[to_update].Remove_From_Number_Of_Treachery_Cards_Of(faction)
      );
      return true;
    }

    //transfers and such
  }
}
