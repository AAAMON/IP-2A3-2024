using dune_library.Treachery_Cards;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public class Special_Faction_Knowledge {
    public IList<Treachery_Card> Treachery_Cards { get; }

    public IList<General> Traitors { get; }

    public IList<General> Discarded_Traitors { get; }

    public Dictionary<Faction, Treachery_Cards_Intel> Treachery_Cards_Intel { get; }

    public Dictionary<Faction, Traitors_Intel> Traitors_Intel { get; }

    public Dictionary<Faction, Traitors_Intel> Discarded_Traitors_Intel { get; }

    public Special_Faction_Knowledge() {
      Treachery_Cards = [];
      Traitors = [];
      Discarded_Traitors = [];
      Treachery_Cards_Intel = [];
      Traitors_Intel = [];
      Discarded_Traitors_Intel = [];
    }

    public Special_Faction_Knowledge(
      IList<Treachery_Card> treachery_cards,
      IList<General> traitors,
      IList<General> discarded_traitors,
      Dictionary<Faction, Treachery_Cards_Intel> treachery_cards_intel,
      Dictionary<Faction, Traitors_Intel> traitors_intel,
      Dictionary<Faction, Traitors_Intel> discarded_traitors_intel
    ) {
      Treachery_Cards = treachery_cards;
      Traitors = traitors;
      Discarded_Traitors = discarded_traitors;
      Treachery_Cards_Intel = treachery_cards_intel;
      Traitors_Intel = traitors_intel;
      Discarded_Traitors_Intel = discarded_traitors_intel;
    }
  }
}
