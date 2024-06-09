using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dune_library.Utils;
using EnumsNET;
using static dune_library.Decks.Treachery.Treachery_Cards;

namespace dune_library.Decks.Treachery {
  public class Treachery_Deck {
    public Treachery_Deck() {
      Discard_Pile = Default_Treachery_Deck_Composition.Clone();
      Card_Stack = [];
      rng = new Random();
    }

    private readonly Random rng;

    private I_Occurence_Dict<Treachery_Card> Discard_Pile { get; }

    private Stack<Treachery_Card> Card_Stack { get; }

    private void Shuffle_Discard_Pile_Cards_And_Place_Them_In_Card_Stack() {
      var index_to_transfer = rng.Next(Discard_Pile.Count);
      var card_to_transfer = Discard_Pile.Keys.ElementAt(index_to_transfer);
      Discard_Pile.Remove(card_to_transfer, 1);
      Card_Stack.Push(card_to_transfer);
    }

    public Treachery_Card Next_Card_Peek {
      get {
        if (Card_Stack.Count == 0) {
          Shuffle_Discard_Pile_Cards_And_Place_Them_In_Card_Stack();
        }
        return Card_Stack.Peek();
      }
    }

    public Treachery_Card Take_Next_Card() {
      if (Card_Stack.Count == 0) {
        Shuffle_Discard_Pile_Cards_And_Place_Them_In_Card_Stack();
      }
      return Card_Stack.Pop();
    }

    public void Add_To_Discard_Pile(Treachery_Card to_discard) {
      if (to_discard.Has_To_Be_Removed_From_The_Game_After_Use() == true) {
        return;
      }
      Discard_Pile.Add(to_discard);
    }
  }
}
