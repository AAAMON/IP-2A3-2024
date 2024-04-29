using dune_library.Player_Resources;
using dune_library.Utils;
using LanguageExt.ClassInstances.Pred;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Map_Resoures {
  public class Local_Faction_Presences {
    public Local_Faction_Presences() {
      Presences = new Dictionary<Faction, Forces_Container>();
    }

    [JsonConstructor]
    public Local_Faction_Presences(IDictionary<Faction, Forces_Container> presences) {
      Presences = presences;
    }

    [JsonInclude]
    private IDictionary<Faction, Forces_Container> Presences { get; }

    public Forces_Container Of(Faction faction) {
      Throw_If_Not_Present(faction);
      return Presences[faction].Clone;
    }

    public bool Is_Present(Faction faction) => Presences.ContainsKey(faction);

    private void Throw_If_Not_Present(Faction faction) {
      if (!Is_Present(faction)) {
        throw new ArgumentException("This faction (" + faction + ") is not present in this section", nameof(faction));
      }
    }

    [JsonIgnore]
    public int Number_Of_Factions_Present => Presences.Count;

    #region Add, Bene Gesserit swaps and Remove

    public void Add(Faction faction, uint normal, uint special) {
      if (Is_Present(faction)) {
        Presences[faction].Add(normal, special);
      } else {
        Presences[faction] = (normal, special);
      }
    }

    public void Flip_Normal_To_Special(Faction faction) {
      Throw_If_Not_Present(faction);
      Presences[faction].Flip_Normal_To_Special();
    }

    public void Flip_Special_To_Normal(Faction faction) {
      Throw_If_Not_Present(faction);
      Presences[faction].Flip_Special_To_Normal();
    }

    public void Swap(Faction faction) {
      Throw_If_Not_Present(faction);
      Presences[faction].Swap();
    }

    public void Remove(Faction faction, uint normal, uint special) {
      Throw_If_Not_Present(faction);
      Presences[faction].Remove(normal, special);
      if (Presences[faction].Is_Empty) {
        Presences.Remove(faction);
      }
    }

    #endregion
  }
}
