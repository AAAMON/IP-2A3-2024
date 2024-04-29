using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public class Public_Faction_Knowledge_Manager {
    public Public_Faction_Knowledge_Manager(ISet<Faction> factions) {
      Public_Faction_Knowledge = factions.Select(faction =>
        new KeyValuePair<Faction, Public_Faction_Knowledge>(faction, new(faction, 0, 0, 20))
      ).ToDictionary();
    }

    [JsonConstructor]
    public Public_Faction_Knowledge_Manager(IDictionary<Faction, Public_Faction_Knowledge> public_Faction_Knowledge) {
      Public_Faction_Knowledge = public_Faction_Knowledge;
    }

    [JsonInclude]
    private IDictionary<Faction, Public_Faction_Knowledge> Public_Faction_Knowledge { get; }

    public Public_Faction_Knowledge Of(Faction faction) => Public_Faction_Knowledge[faction];
  }
}
