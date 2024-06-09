using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static dune_library.Player_Resources.Factions_Distribution_Manager;
using static dune_library.Utils.Exceptions;

namespace dune_library.Player_Resources {
  public class Final_Factions_Distribution {
    public Final_Factions_Distribution(IReadOnlySet<Player> players, Factions_Distribution_Manager faction_manager) {
      Player_To_Faction = players.Select(player =>
        new KeyValuePair<Player, Faction>(player, faction_manager.Faction_Of(player).Value())
      ).ToFrozenDictionary();
      Factions_In_Play = Player_To_Faction.Values.ToFrozenSet();
      Faction_To_Player = Factions_In_Play.Select(faction =>
        new KeyValuePair<Faction, Player>(faction, faction_manager.Player_Of(faction).ValueUnsafe())
      ).ToFrozenDictionary();
    }

    private IReadOnlyDictionary<Player, Faction> Player_To_Faction { get; }

    public Faction Faction_Of(Player player) {
      if (Player_To_Faction.ContainsKey(player) == false) {
        throw new Player_Not_In_Game(player);
      }
      return Player_To_Faction[player];
    }

    private IDictionary<Faction, Player> Faction_To_Player { get; }

    public Player Player_Of(Faction faction) {
      if (Faction_To_Player.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      return Faction_To_Player[faction];
    }

    public IReadOnlySet<Faction> Factions_In_Play { get; }
  }
}
