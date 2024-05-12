using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.UnsafeValueAccess;
using System.Collections.Frozen;
using static dune_library.Utils.Exceptions;
using System.Text.Json.Serialization;

namespace dune_library.Player_Resources {
  // after player markers initialization, all players have a player marker position
  public class Player_Markers {

    #region Exceptions

    public class Factions_Without_Player_Markers_Exist : InvalidOperationException {
      public IEnumerable<Faction> Factions_Without_Player_Markers { get; }
      public Factions_Without_Player_Markers_Exist(IEnumerable<Faction> factions) :
        base("the following factions don't have a player marker assigned: " + factions) {
        Factions_Without_Player_Markers = factions;
      }
    }

    public class Player_Marker_Initialization_Ended : InvalidOperationException {
      public Player_Marker_Initialization_Ended() :
        base("The time to change the player marker positions has passed") { }
    }

    public class Player_Marker_Initialization_Did_Not_End : InvalidOperationException {
      public Player_Marker_Initialization_Did_Not_End() :
        base("This operation is valid only after the player marker positions initialization has ended") { }
    }

    public class Invalid_Player_Marker_Position : ArgumentException {
      public Invalid_Player_Marker_Position(uint position, IEnumerable<uint> valid_positions) :
        base("Invalid player marker position (position: " + position + ", valid positions: " + valid_positions + ")") { }
    }

    public class Player_Marker_Position_Is_Taken : ArgumentException {
      public Player_Marker_Position_Is_Taken(uint position) :
        base("this player marker position (" + position + ") is already taken") { }
    }

    public class Faction_Has_No_Player_Marker_Position : ArgumentException {
      public Faction_Has_No_Player_Marker_Position(Faction faction) :
        base("Faction " + faction + " does not have a player marker position") { }
    }

    public class Faction_Has_A_Player_Marker_Position : ArgumentException {
      public Faction_Has_A_Player_Marker_Position(Faction faction) :
        base("Faction " + faction + " already has a player marker position") { }
    }

    #endregion

    #region Stop conditions

    private bool in_player_markers_initialization;

    public void End_Player_Markers_Initialization() {
      var factions_without_player_markers = Faction_To_Marker.Where(kvp => kvp.Value == None).Select(kvp => kvp.Key);
      if (factions_without_player_markers.Count() > 0) {
        throw new Factions_Without_Player_Markers_Exist(factions_without_player_markers);
      }
      in_player_markers_initialization = false;
    }

    #endregion

    [JsonInclude]
    private IDictionary<Faction, Option<uint>> Faction_To_Marker { get; }

    public bool Has_Player_Marker(Faction faction) {
      if (Faction_To_Marker.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      return Faction_To_Marker[faction].IsSome;
    }

    public uint Marker_Of(Faction faction) {
      if (in_player_markers_initialization) {
        throw new Player_Marker_Initialization_Did_Not_End();
      }
      if (Faction_To_Marker.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      return Faction_To_Marker[faction].Value();
    }

    [JsonInclude]
    private IDictionary<uint, Option<Faction>> Marker_To_Faction { get; }

    public Option<Faction> Faction_At(uint position) {
      if (in_player_markers_initialization) {
        throw new Player_Marker_Initialization_Did_Not_End();
      }
      if (Marker_To_Faction.ContainsKey(position) == false) {
        throw new Invalid_Player_Marker_Position(position, Marker_To_Faction.Keys);
      }
      return Marker_To_Faction[position];
    }

    [JsonIgnore]
    public IEnumerable<uint> Free_Player_Markers =>
      Marker_To_Faction.Where(kvp => kvp.Value.IsNone).Select(kvp => kvp.Key);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    // Wanted to assign default values for default_marker_positions if none were passed
    // but they need to be known at compile time and, apparently, that can't be done
    public Player_Markers(IReadOnlySet<Faction> factions_in_play, IReadOnlySet<uint> default_marker_positions = null) {
      in_player_markers_initialization = true;
      if (default_marker_positions is null) {
        default_marker_positions = new System.Collections.Generic.HashSet<uint>() { 1, 4, 7, 10, 13, 16 };
      }
      Faction_To_Marker = factions_in_play.Select(faction =>
        new KeyValuePair<Faction, Option<uint>>(faction, None)
      ).ToDictionary();
      Marker_To_Faction = default_marker_positions.Select(position =>
        new KeyValuePair<uint, Option<Faction>>(position, None)
      ).ToDictionary();
    }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

    // faction takes position
    public void Assign_Player_Marker(Faction faction, uint position) {
      if (in_player_markers_initialization == false) {
        throw new Player_Marker_Initialization_Ended();
      }
      if (Faction_To_Marker.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      if (Marker_To_Faction.ContainsKey(position) == false) {
        throw new Invalid_Player_Marker_Position(position, Marker_To_Faction.Keys);
      }
      lock (this) {
        if (Marker_To_Faction[position].IsSome) {
          throw new Player_Marker_Position_Is_Taken(position);
        }
        if (Faction_To_Marker[faction].IsSome) {
          Marker_To_Faction[Faction_To_Marker[faction].Value()] = None;
        }
        Faction_To_Marker[faction] = position;
        Marker_To_Faction[position] = faction;
      }
    }

    // a and b swap positions
    public void Trade_Player_Markers(Faction a, Faction b) {
      if (in_player_markers_initialization == false) {
        throw new Player_Marker_Initialization_Ended();
      }
      if (Faction_To_Marker.ContainsKey(a) == false) {
        throw new Faction_Not_In_Play(a);
      }
      if (Faction_To_Marker.ContainsKey(b) == false) {
        throw new Faction_Not_In_Play(b);
      }
      lock (this) {
        if (Faction_To_Marker[a].IsNone) {
          throw new Faction_Has_No_Player_Marker_Position(a);
        }
        if (Faction_To_Marker[b].IsNone) {
          throw new Faction_Has_No_Player_Marker_Position(b);
        }
        (Faction_To_Marker[a], Faction_To_Marker[b]) = (Faction_To_Marker[b], Faction_To_Marker[a]);
        Marker_To_Faction[Faction_To_Marker[a].Value()] = a;
        Marker_To_Faction[Faction_To_Marker[b].Value()] = b;
      }
    }

    // transfers position from b to a
    public void Take_Player_Marker(Faction a, Faction b) {
      if (in_player_markers_initialization == false) {
        throw new Player_Marker_Initialization_Ended();
      }
      if (Faction_To_Marker.ContainsKey(a) == false) {
        throw new Faction_Not_In_Play(a);
      }
      if (Faction_To_Marker.ContainsKey(b) == false) {
        throw new Faction_Not_In_Play(b);
      }
      lock (this) {
        if (Faction_To_Marker[a].IsSome) {
          throw new Faction_Has_A_Player_Marker_Position(a);
        }
        if (Faction_To_Marker[b].IsNone) {
          throw new Faction_Has_No_Player_Marker_Position(b);
        }
        (Faction_To_Marker[a], Faction_To_Marker[b]) = (Faction_To_Marker[b], None);
        Marker_To_Faction[Faction_To_Marker[a].Value()] = a;
      }
    }
  }
}