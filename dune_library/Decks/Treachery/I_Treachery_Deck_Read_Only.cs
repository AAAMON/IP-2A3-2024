using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static dune_library.Decks.Treachery.Treachery_Cards;

namespace dune_library.Decks.Treachery {
  public interface I_Treachery_Deck_Read_Only {
    public Treachery_Card Next_Card_Peek { get; }
  }
}
