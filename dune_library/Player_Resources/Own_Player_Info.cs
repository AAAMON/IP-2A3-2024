using dune_library.Treachery_Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  internal class Own_Player_Info : Player_Info {
    public Own_Player_Info(int spice, int reserves, int dead_forces, IList<General> generals,
      IList<General> traitors, IList<General> discarded_traitors, IList<Treachery_Card> treachery_cards) :
      base(spice, reserves, dead_forces, generals, traitors, discarded_traitors) {
      Treachery_Cards = treachery_cards;
    }

    public IList<Treachery_Card> Treachery_Cards { get; set; }
  }
}
