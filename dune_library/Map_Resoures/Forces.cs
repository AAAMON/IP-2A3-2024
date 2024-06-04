using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dune_library.Utils;
using dune_library.Player_Resources;
using System.Text.Json.Serialization;

namespace dune_library.Map_Resoures {
  public class Forces {
    public Forces() {
      Forces_Dict = new Occurence_Dict<Faction>();
    }

    public static Forces Initial_Reserves_From(IReadOnlySet<Faction> factions_in_play) => new(new Occurence_Dict<Faction>(
      factions_in_play.Select(faction => new KeyValuePair<Faction, uint>(faction, 20))
    ));

    public Forces(I_Occurence_Dict<Faction> forces_dict) {
      Forces_Dict = forces_dict;
    }

    public uint this[Faction faction] {
      init => Forces_Dict[faction] = value;
    }

    private I_Occurence_Dict<Faction> Forces_Dict { get; }

    #region Serialization stuff

    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    private uint? Atreides => Forces_Dict.ContainsKey(Faction.Atreides) ? Forces_Dict[Faction.Atreides] : null;
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    private uint? Bene_Gesserit => Forces_Dict.ContainsKey(Faction.Bene_Gesserit) ? Forces_Dict[Faction.Bene_Gesserit] : null;
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    private uint? Emperor => Forces_Dict.ContainsKey(Faction.Emperor) ? Forces_Dict[Faction.Emperor] : null;
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    private uint? Fremen => Forces_Dict.ContainsKey(Faction.Fremen) ? Forces_Dict[Faction.Fremen] : null;
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    private uint? Spacing_Guild => Forces_Dict.ContainsKey(Faction.Spacing_Guild) ? Forces_Dict[Faction.Spacing_Guild] : null;
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    private uint? Harkonnen => Forces_Dict.ContainsKey(Faction.Harkonnen) ? Forces_Dict[Faction.Harkonnen] : null;
    // I don't know a better way of writing this (I tried implementing ISerializable through Forces_Dict

    #endregion

    public uint Of(Faction faction) => Forces_Dict.TryGetValue(faction, out uint value) ? value : 0;

    [JsonIgnore]
    public uint Number_Of_Factions_Present => (uint)Forces_Dict.Keys.Count();

    [JsonIgnore]
    public bool No_Faction_Is_Present => Number_Of_Factions_Present == 0;

    public void Transfer_To(Faction faction, Forces destination, uint to_transfer) {
      if (Forces_Dict.Remove(faction, to_transfer) == false) {
        throw new ArgumentException("not enough forces in source for faction " + faction);
      }
      destination.Forces_Dict.Add(faction, to_transfer);
    }

    public void Transfer_To(Faction faction, Forces destination) {
      if (Forces_Dict.Remove(faction, out uint to_transfer) == false) {
        return;
      }
      destination.Forces_Dict.Add(faction, to_transfer);
    }

    public void Transfer_From(Faction faction, Forces source, uint to_transfer) => source.Transfer_To(faction, this, to_transfer);

    public void Transfer_From(Faction faction, Forces source) => source.Transfer_To(faction, this);

    public void Remove_By_Storm(Tleilaxu_Tanks graveyard) {
      Forces_Dict.Keys.ForEach(faction => {
        Transfer_To(faction, graveyard.Forces);
      });
    }

    public void Remove_By_Shai_Hulud(Tleilaxu_Tanks graveyard) {
      Forces_Dict.Keys.ForEach(faction => {
          if(faction != Faction.Fremen)
          {
            Transfer_To(faction, graveyard.Forces);
          }
      });
    }

    public bool Is_Present(Faction faction) => Forces_Dict.ContainsKey(faction);
  }
}
