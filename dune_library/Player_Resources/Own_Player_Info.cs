using dune_library.Treachery_Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  internal class Own_Player_Info : Player_Info {
    public IList<Treachery_Card> Treachery_Cards { get; set; }
  }
}
