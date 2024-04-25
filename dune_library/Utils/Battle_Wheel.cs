using dune_library.Player_Resources;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils {
  internal class Battle_Wheel {
    public Battle_Wheel() { }

    public int Get_from_range_closed(int min, int max) {
      var result = max + 1;
      // while (result < min || result > max) {
      //   ask player for a number between min and max
      //   get number from player
      // }
      result = min;
      return result;
    }

    public Option<Player> Last_Player { get; set; }
  }
}
