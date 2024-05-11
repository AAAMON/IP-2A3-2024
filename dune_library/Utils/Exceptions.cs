using dune_library.Player_Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils {
  public static class Exceptions {
    public class Faction_Not_In_Play : InvalidOperationException {
      public Faction_Not_In_Play(Faction faction) : base("Faction " + faction + " is not in play") { }
    }
  }
}
