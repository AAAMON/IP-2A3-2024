using dune_library.Utils;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using LanguageExt.UnsafeValueAccess;
using static dune_library.Utils.Exceptions;
using System.Collections.Immutable;
using System.Collections.Frozen;

namespace dune_library.Player_Resources {
  public class Factions_Distribution_Manager {

    public Factions_Distribution_Manager(IReadOnlySet<Player> players) {
      Player_To_Faction = players.Select(player =>
        new KeyValuePair<Player, Option<Faction>>(player, None)
      ).ToDictionary();
      Faction_To_Player = Enum.GetValues<Faction>().Select(faction =>
        new KeyValuePair<Faction, Option<Player>>(faction, None)
      ).ToDictionary();
    }

    public bool Can_Convert_To_Final => Player_To_Faction.All(kvp => kvp.Value.IsSome);

    #region Exceptions

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

    private IDictionary<Player, Option<Faction>> Player_To_Faction { get; }

    public Option<Faction> Faction_Of(Player player) {
      if (Player_To_Faction.ContainsKey(player) == false) {
        throw new Player_Not_In_Game(player);
      }
      return Player_To_Faction[player];
    }

    private IDictionary<Faction, Option<Player>> Faction_To_Player { get; }

    public Option<Player> Player_Of(Faction faction) => Faction_To_Player[faction];

    public IReadOnlySet<Faction> Free_Factions => Faction_To_Player.Where(kvp => kvp.Value.IsNone).Select(kvp => kvp.Key).ToHashSet();

    public IReadOnlySet<Faction> Taken_Factions => Faction_To_Player.Where(kvp => kvp.Value.IsSome).Select(kvp => kvp.Key).ToHashSet();

    // player takes faction
    public void Assign_Faction(Player player, Faction faction) {
      if (Player_To_Faction.ContainsKey(player) == false) {
        throw new Player_Not_In_Game(player);
      }
      if (Faction_To_Player[faction].IsSome) {
        throw new Faction_Is_Taken(faction);
      }
      if (Player_To_Faction[player].IsSome) {
        Faction_To_Player[Player_To_Faction[player].ValueUnsafe()] = None;
      }
      Player_To_Faction[player] = faction;
      Faction_To_Player[faction] = player;
    }

    // a and b swap factions
    public void Trade_Factions(Player a, Player b) {
      if (Player_To_Faction.ContainsKey(a) == false) {
        throw new Player_Not_In_Game(a);
      }
      if (Player_To_Faction.ContainsKey(b) == false) {
        throw new Player_Not_In_Game(b);
      }
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

    // doesn't make much sense now
    // transfers faction from b to a
    public void Take_Faction(Player a, Player b) {
      if (Player_To_Faction.ContainsKey(a) == false) {
        throw new Player_Not_In_Game(a);
      }
      if (Player_To_Faction.ContainsKey(b) == false) {
        throw new Player_Not_In_Game(b);
      }
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
