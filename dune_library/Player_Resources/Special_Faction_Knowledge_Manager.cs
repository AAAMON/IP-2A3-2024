using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public class Special_Faction_Knowledge_Manager {
    public Special_Faction_Knowledge_Manager(ISet<Faction> factions) {
      Special_Faction_Knowledge = factions.Select(faction =>
        new KeyValuePair<Faction, Special_Faction_Knowledge>(faction, new(factions))
      ).ToDictionary();
    }

    private IDictionary<Faction, Special_Faction_Knowledge> Special_Faction_Knowledge { get; }

    public Special_Faction_Knowledge Of(Faction faction) => Special_Faction_Knowledge[faction];
  }
}
