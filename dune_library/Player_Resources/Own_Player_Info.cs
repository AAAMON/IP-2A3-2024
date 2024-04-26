using dune_library.Treachery_Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  internal class Own_Player_Info(IList<General> generals, IList<General> traitors, IList<General> discarded_traitors,
    IList<Treachery_Card> treachery_cards) : Player_Info(generals, traitors, discarded_traitors) {
    public IList<Treachery_Card> Treachery_Cards { get; set; } = treachery_cards;
  }
}
