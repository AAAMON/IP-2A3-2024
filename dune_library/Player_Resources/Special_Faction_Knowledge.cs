using dune_library.Treachery_Cards;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public class Special_Faction_Knowledge {
    public uint Spice { get; private set; }

    public IList<Treachery_Card> Treachery_Cards { get; }

    public IList<General> Traitors { get; }

    public IList<General> Discarded_Traitors { get; }

    public IDictionary<Faction, uint> Number_Of_Treachery_Cards_Of_Other_Players { get; }

    public Special_Faction_Knowledge(IEnumerable<Faction> factions_in_play) {
      Treachery_Cards = [];
      Traitors = [];
      Discarded_Traitors = [];
      Number_Of_Treachery_Cards_Of_Other_Players = new Dictionary<Faction, uint>();
      Spice = 0;
    }

    public void Add_Spice(uint to_add) => Spice += to_add;

    public bool Remove_Spice(uint to_remove) {
      return false;
    }
  }
}
