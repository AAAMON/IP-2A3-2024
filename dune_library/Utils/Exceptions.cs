using dune_library.Player_Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils {
  public static class Exceptions {
    public class Faction_Selection_Ongoing : InvalidOperationException {
      public Faction_Selection_Ongoing() :
        base("The faction selection process is still ongoing") { }
    }

    public class Player_Marker_Selection_Ongoing : InvalidOperationException {
      public Player_Marker_Selection_Ongoing() :
        base("The player marker selection process is still ongoing") { }
    }

    public class Player_Not_In_Game : ArgumentException {
      public Player_Not_In_Game(Player player) :
        base("Player " + player + " is not in game") { }
    }

    public class Faction_Not_In_Play : InvalidOperationException {
      public Faction_Not_In_Play(Faction faction) : base("Faction " + faction + " is not in play") { }
    }

    public class Faction_Is_Not_Fully_Implemented : NotImplementedException {
      public Faction_Is_Not_Fully_Implemented(Faction faction) : base("Faction " + faction + " is not fully implemented") { }
    }

    public class Variable_Is_Not_Initialized : InvalidOperationException {
      public Variable_Is_Not_Initialized(object variable) :
        base("Object " + variable + " has not been initialized yet") { }
    }
  }
}
