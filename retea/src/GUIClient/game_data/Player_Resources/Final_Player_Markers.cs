using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static dune_library.Player_Resources.Player_Markers_Manager;
using static dune_library.Utils.Exceptions;

namespace dune_library.Player_Resources {
  public class Final_Player_Markers {
    public Final_Player_Markers(IReadOnlySet<Faction> factions_at_play, Player_Markers_Manager player_markers_manager) {
      Faction_To_Marker = factions_at_play.Select(faction => 
        new KeyValuePair<Faction, uint>(faction, player_markers_manager.Marker_Of(faction).Value())
      ).ToFrozenDictionary();
      Player_Marker_Positions = Faction_To_Marker.Values.ToImmutableList();
      Marker_To_Faction = Player_Marker_Positions.Select(position =>
        new KeyValuePair<uint, Faction>(position, player_markers_manager.Faction_At(position).Value())
      ).ToFrozenDictionary();
    }

    [JsonInclude]
    private IReadOnlyDictionary<Faction, uint> Faction_To_Marker { get; }

    public uint Marker_Of(Faction faction) {
      if (Faction_To_Marker.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      return Faction_To_Marker[faction];
    }

    [JsonInclude]
    private IReadOnlyDictionary<uint, Faction> Marker_To_Faction { get; }

    public Faction Faction_At(uint position) {
      if (Marker_To_Faction.ContainsKey(position) == false) {
        throw new Invalid_Player_Marker_Position(position, Marker_To_Faction.Keys);
      }
      return Marker_To_Faction[position];
    }

    public IReadOnlyList<uint> Player_Marker_Positions { get; }
  }
}
