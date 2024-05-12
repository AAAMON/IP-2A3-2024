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

namespace dune_library.Player_Resources {
  public class Alliances {
    #region Conditions for altering state

    private bool is_in_nexus;

    public void Enter_Nexus() => is_in_nexus = true;

    public void Exit_Nexus() => is_in_nexus = false;

    public class Not_In_Nexus : InvalidOperationException {
      public Not_In_Nexus() : base("this operation can not be attempted if not in nexus") { }
    }

    #endregion

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

    public Alliances(IReadOnlySet<Faction> factions_in_play) {
      Ally_Dict = factions_in_play.Select(faction =>
        new KeyValuePair<Faction, Option<Faction>>(faction, None)
      ).ToDictionary();
      is_in_nexus = false;
    }

    public class Already_Has_An_Ally : ArgumentException {
      public Already_Has_An_Ally(Faction faction, Faction ally) :
        base("This faction (" + faction + ") is already allied to another faction (" + ally + ")") { }
    }

    public void Ally(Faction a, Faction b) {
      if (is_in_nexus == false) {
        throw new Not_In_Nexus();
      }
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

    public class Doesn_t_Have_An_Ally : ArgumentException {
      public Doesn_t_Have_An_Ally(Faction faction) :
        base("This faction (" + faction + ") doesn't have an ally") { }
    }

    public class Are_Not_Allied : ArgumentException {
      public Are_Not_Allied(Faction a, Faction ally_of_a, Faction b, Faction ally_of_b) :
        base("These two factions (" + a + " and " + b + ") are not allied " +
          "(" + a + "'s ally: " + ally_of_a + ", " + b + "'s ally: " + ally_of_b + ")") { }
    }

    public void Break_Alliance(Faction a, Faction b) {
      if (is_in_nexus == false) {
        throw new Not_In_Nexus();
      }
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
      if (is_in_nexus == false) {
        throw new Not_In_Nexus();
      }
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
