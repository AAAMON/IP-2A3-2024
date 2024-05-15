using dune_library.Map_Resoures;
using dune_library.Utils;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static dune_library.Utils.Extensions;
using LanguageExt.UnsafeValueAccess;

namespace dune_library.Decks.Spice {
  public class Spice_Deck {
    public Spice_Deck(Map_Resources.Map map) {
      Discard_Pile = [];

      IEnumerable<Territory_Card> territory_cards = Range(0, (uint)map.Sections_With_Spice.Count).Select(i => new Territory_Card(i));

      var shai_hulud_producer = () => new Shai_Hulud_Card();
      IEnumerable<Shai_Hulud_Card> shai_hulud_cards = shai_hulud_producer.Repeat(6);

      IList<Spice_Card> all_cards = [.. territory_cards, .. shai_hulud_cards];

      all_cards.Shuffle();

      Card_Stack = [];
      all_cards.ForEach(Card_Stack.Push);
    }

    private IList<Spice_Card> Discard_Pile { get; }

    private Stack<Spice_Card> Card_Stack { get; }

    public Option<Spice_Card> Top_OF_Discard_Pile => Discard_Pile.Count == 0 ? None : Discard_Pile.Last();

    public Spice_Card Take_Next_Card() {
      if (Card_Stack.Count == 0) {
        Discard_Pile.Shuffle();
        Discard_Pile.ForEach(Card_Stack.Push);
      }
      return Card_Stack.Pop();
    }

    public void Add_To_Discard_Pile(Spice_Card card) {
      Discard_Pile.Add(card);
    }
  }
}
