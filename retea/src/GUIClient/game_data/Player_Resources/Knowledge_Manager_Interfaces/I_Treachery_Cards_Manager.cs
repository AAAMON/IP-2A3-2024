using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static dune_library.Decks.Treachery.Treachery_Cards;

namespace dune_library.Player_Resources.Knowledge_Manager_Interfaces {
  public interface I_Treachery_Cards_Manager {
    public class Faction_Does_Not_Own_This_Treachery_Card : ArgumentException {
      public Faction Faction { get; }

      public I_Occurence_Dict_Read_Only<Treachery_Card> Treachery_Cards_Owned { get; }

      public Treachery_Card Treachery_Card_Requested { get; }

      public Faction_Does_Not_Own_This_Treachery_Card(Faction faction, I_Occurence_Dict_Read_Only<Treachery_Card> owned, Treachery_Card requested) :
        base("Faction (" + faction + ") does not own the requested treachery card (treachery cards owned: " + owned + ", treachery card requested: " + requested + ")") {
        Faction = faction;
        Treachery_Cards_Owned = owned;
        Treachery_Card_Requested = requested;
      }
    }
    public void Give_A_Treachery_Card(Faction faction);

    public void Remove_Treachery_Card(Faction faction, Treachery_Card to_remove);
  }
}
