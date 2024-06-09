using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using LanguageExt.UnsafeValueAccess;
using dune_library.Utils;
using static dune_library.Utils.Exceptions;
using static dune_library.Player_Resources.I_Alliances;

namespace dune_library.Player_Resources {
  public class Alliances : I_Alliances {
    public Alliances(IReadOnlySet<Faction> factions_in_play) {
      Ally_Dict = factions_in_play.Select(faction =>
        new KeyValuePair<Faction, Option<Faction>>(faction, None)
      ).ToDictionary();
    }

    private IDictionary<Faction, Option<Faction>> Ally_Dict { get; }

    #region Serialization stuff

    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    private Option<Faction>? Atreides => Ally_Dict.ContainsKey(Faction.Atreides) ? Ally_Dict[Faction.Atreides] : null;
    
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    private Option<Faction>? Bene_Gesserit => Ally_Dict.ContainsKey(Faction.Bene_Gesserit) ? Ally_Dict[Faction.Bene_Gesserit] : null;
    
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    private Option<Faction>? Emperor => Ally_Dict.ContainsKey(Faction.Emperor) ? Ally_Dict[Faction.Emperor] : null;
    
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    private Option<Faction>? Fremen => Ally_Dict.ContainsKey(Faction.Fremen) ? Ally_Dict[Faction.Fremen] : null;
    
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    private Option<Faction>? Spacing_Guild => Ally_Dict.ContainsKey(Faction.Spacing_Guild) ? Ally_Dict[Faction.Spacing_Guild] : null;
    
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    private Option<Faction>? Harkonnen => Ally_Dict.ContainsKey(Faction.Harkonnen) ? Ally_Dict[Faction.Harkonnen] : null;

    #endregion

    public Option<Faction> Ally_Of(Faction faction) {
      if (Ally_Dict.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      return Ally_Dict[faction];
    }

    public void Ally(Faction a, Faction b) {
      if (Ally_Dict.ContainsKey(a) == false) {
        throw new Faction_Not_In_Play(a);
      }
      if (Ally_Dict.ContainsKey(b) == false) {
        throw new Faction_Not_In_Play(b);
      }
      lock (this) {
        if (Ally_Dict[a].IsSome) { throw new Already_Has_An_Ally(a, Ally_Dict[a].Value()); }
        if (Ally_Dict[b].IsSome) { throw new Already_Has_An_Ally(b, Ally_Dict[b].Value()); }
        Ally_Dict[a] = b;
        Ally_Dict[b] = a;
      }
    }

    public void Break_Alliance(Faction a, Faction b) {
      if (Ally_Dict.ContainsKey(a) == false) {
        throw new Faction_Not_In_Play(a);
      }
      if (Ally_Dict.ContainsKey(b) == false) {
        throw new Faction_Not_In_Play(b);
      }
      lock (this) {
        if (Ally_Dict[a].IsSome) { throw new Doesn_t_Have_An_Ally(a); }
        if (Ally_Dict[b].IsSome) { throw new Doesn_t_Have_An_Ally(b); }
        if (Ally_Dict[a].Value() != b || Ally_Dict[b].Value() != a) {
          throw new Are_Not_Allied(a, Ally_Dict[a].Value(), b, Ally_Dict[b].Value());
        }
        Ally_Dict[a] = None;
        Ally_Dict[b] = None;
      }
    }

    public void Break_Alliance(Faction faction) {
      if (Ally_Dict.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      lock (this) {
        Faction former_ally = Ally_Dict[faction].OrElseThrow(new Doesn_t_Have_An_Ally(faction));
        if (Ally_Dict[former_ally].IsSome) { throw new Doesn_t_Have_An_Ally(former_ally); }
        if (Ally_Dict[former_ally].Value() != faction) {
          throw new Are_Not_Allied(former_ally, Ally_Dict[former_ally].Value(), faction, Ally_Dict[faction].Value());
        }
        Ally_Dict[faction] = None;
        Ally_Dict[former_ally] = None;
      }
    }
  }
}
