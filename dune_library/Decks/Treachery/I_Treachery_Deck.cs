using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static dune_library.Decks.Treachery.Treachery_Cards;

namespace dune_library.Decks.Treachery {
  public interface I_Treachery_Deck : I_Treachery_Deck_Read_Only {
    public Treachery_Card Take_Next_Card();

    public void Add_To_Discard_Pile(Treachery_Card to_discard);
  }
}
