using dune_library.Utils;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Reactive.Linq;
using LanguageExt.UnsafeValueAccess;
using static dune_library.Player_Resources.Player_Markers;
using static dune_library.Utils.Exceptions;
using System.Collections.Immutable;

namespace dune_library.Player_Resources {
  public class Factions_Manager {

    #region Exceptions

    public class Players_Without_Factions_Exist : InvalidOperationException {
      public IEnumerable<Player> Players_Without_Factions { get; }
      public Players_Without_Factions_Exist(IEnumerable<Player> players) :
        base("the following players don't have a faction assigned: " + players) {
        Players_Without_Factions = players;
      }
    }

    public class Faction_Selection_Ended : InvalidOperationException {
      public Faction_Selection_Ended() :
        base("The faction selection period has ended") { }
    }

    public class Faction_Selection_Ongoing : InvalidOperationException {
      public Faction_Selection_Ongoing() :
        base("The faction selection process is still ongoing") { }
    }

    public class Player_Not_In_Game : ArgumentException {
      public Player_Not_In_Game(Player player) :
        base("Player " + player + " is not in game") { }
    }

    public class Faction_Is_Taken : ArgumentException {
      public Faction_Is_Taken(Faction faction) :
        base("this faction (" + faction + ") is already taken") { }
    }

    public class Player_Has_No_Faction : ArgumentException {
      public Player_Has_No_Faction(Player player) :
        base("Player " + player + " does not have a faction") { }
    }

    public class Player_Has_A_Faction : ArgumentException {
      public Player_Has_A_Faction(Player player) :
        base("Player " + player + " already has a faction") { }
    }

    #endregion

    #region Stop conditions

    private bool players_can_choose_factions;

    public void Mark_Faction_Selection_End() {
      var players_without_factions = Player_To_Faction.Where(kvp => kvp.Value == None).Select(kvp => kvp.Key);
      if (players_without_factions.Count() > 0) {
        throw new Players_Without_Factions_Exist(players_without_factions);
      }
      factions_in_play = Player_To_Faction.Select(kvp => kvp.Value.Value()).ToImmutableHashSet();
      players_can_choose_factions = false;
    }

    #endregion

    private IDictionary<Player, Option<Faction>> Player_To_Faction { get; }

    public bool Has_Faction(Player player) {
      if (Player_To_Faction.ContainsKey(player) == false) {
        throw new Player_Not_In_Game(player);
      }
      return Player_To_Faction[player].IsSome;
    }

    public Faction Faction_Of(Player player) {
      if (players_can_choose_factions) {
        throw new Faction_Selection_Ongoing();
      }
      if (Player_To_Faction.ContainsKey(player) == false) {
        throw new Player_Not_In_Game(player);
      }
      return Player_To_Faction[player].Value();
    }

    private IDictionary<Faction, Option<Player>> Faction_To_Player { get; }

    public Option<Player> Player_Of(Faction faction) {
      if (players_can_choose_factions) {
        throw new Faction_Selection_Ongoing();
      }
      return Faction_To_Player[faction];
    }

    [JsonIgnore]
    public IReadOnlySet<Faction> Free_Factions =>
      Faction_To_Player.Where(kvp => kvp.Value.IsNone).Select(kvp => kvp.Key).ToImmutableHashSet();

    private IReadOnlySet<Faction> factions_in_play;
    public IReadOnlySet<Faction> Factions_In_Play {
      get {
        if (players_can_choose_factions) {
          throw new Faction_Selection_Ongoing();
        }
        return factions_in_play;
      }
    }

    public Factions_Manager(ISet<Player> players) {
      players_can_choose_factions = true;
      Player_To_Faction = players.Select(player =>
        new KeyValuePair<Player, Option<Faction>>(player, None)
      ).ToDictionary();
      Faction_To_Player = Enum.GetValues<Faction>().Select(faction =>
        new KeyValuePair<Faction, Option<Player>>(faction, None)
      ).ToDictionary();
    }

    // player takes faction
    public void Assign_Faction(Player player, Faction faction) {
      if (players_can_choose_factions == false) {
        throw new Faction_Selection_Ended();
      }
      if (Player_To_Faction.ContainsKey(player) == false) {
        throw new Player_Not_In_Game(player);
      }
      lock (this) {
        if (Faction_To_Player[faction].IsSome) {
          throw new Faction_Is_Taken(faction);
        }
        if (Player_To_Faction[player].IsSome) {
          Faction_To_Player[Player_To_Faction[player].ValueUnsafe()] = None;
        }
        Player_To_Faction[player] = faction;
        Faction_To_Player[faction] = player;
      }
    }

    // a and b swap factions
    public void Trade_Factions(Player a, Player b) {
      if (players_can_choose_factions == false) {
        throw new Faction_Selection_Ended();
      }
      if (Player_To_Faction.ContainsKey(a) == false) {
        throw new Player_Not_In_Game(a);
      }
      if (Player_To_Faction.ContainsKey(b) == false) {
        throw new Player_Not_In_Game(b);
      }
      lock (this) {
        if (Player_To_Faction[a].IsNone) {
          throw new Player_Has_No_Faction(a);
        }
        if (Player_To_Faction[b].IsNone) {
          throw new Player_Has_No_Faction(b);
        }
        (Player_To_Faction[a], Player_To_Faction[b]) = (Player_To_Faction[b], Player_To_Faction[a]);
        Faction_To_Player[Player_To_Faction[a].Value()] = a;
        Faction_To_Player[Player_To_Faction[b].Value()] = b;
      }
    }

    // transfers faction from b to a
    public void Take_Faction(Player a, Player b) {
      if (players_can_choose_factions == false) {
        throw new Faction_Selection_Ended();
      }
      if (Player_To_Faction.ContainsKey(a) == false) {
        throw new Player_Not_In_Game(a);
      }
      if (Player_To_Faction.ContainsKey(b) == false) {
        throw new Player_Not_In_Game(b);
      }
      lock (this) {
        if (Player_To_Faction[a].IsSome) {
          throw new Player_Has_A_Faction(a);
        }
        if (Player_To_Faction[b].IsNone) {
          throw new Player_Has_No_Faction(b);
        }
        (Player_To_Faction[a], Player_To_Faction[b]) = (Player_To_Faction[b], None);
        Faction_To_Player[Player_To_Faction[a].Value()] = a;
      }
    }
  }
}
