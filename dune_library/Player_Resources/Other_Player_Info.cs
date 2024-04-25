using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  internal class Other_Player_Info : Player_Info {
    public Other_Player_Info(Own_Player_Info other) : base(other) {
      Treachery_Cards = 0;
    }
    public int Treachery_Cards { get; set; }
  }
}
