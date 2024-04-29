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

namespace dune_library.Player_Resources {
  public class Faction_Manager {
    public Faction_Manager(ISet<Player> players) {
      Faction_By_Player = new Dictionary<Player, Option<Faction>>();
      players.ForEach(player => Faction_By_Player[player] = None);
      Player_By_Faction = Enum.GetValues<Faction>().Select(faction =>
        new KeyValuePair<Faction, Option<Player>>(faction, None)
      ).ToDictionary();
    }

    [JsonConstructor]
    public Faction_Manager(IDictionary<Player, Option<Faction>> factions_by_players) {
      Faction_By_Player = factions_by_players;
      Player_By_Faction = Enum.GetValues<Faction>().Select(faction =>
        new KeyValuePair<Faction, Option<Player>>(faction, None)
      ).ToDictionary();
      factions_by_players.ForEach(kvp => {
        if (kvp.Value.IsSome) {
          Player_By_Faction[kvp.Value.Value()] = kvp.Key;
        }
      });
    }

    private IDictionary<Player, Option<Faction>> Faction_By_Player { get; }

    public Option<Faction> Faction_OF(Player player) => Faction_By_Player[player];

    [JsonIgnore]
    private IDictionary<Faction, Option<Player>> Player_By_Faction { get; }

    public Option<Player> Player_Of(Faction faction) => Player_By_Faction[faction];

    [JsonIgnore]
    public IEnumerable<Faction> Free_Factions =>
      Enum.GetValues<Faction>().Filter(faction => Player_By_Faction[faction].IsNone);

    /*public void Give_Faction_To_Player(Player player, Faction faction) {

    }

    public void Trade(Player a, Player b) {
      if (Faction_By_Player[a].IsNone) {

      }
    }*/
  }
}
