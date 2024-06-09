using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public interface I_Alliances : I_Alliances_Read_Only {
    public class Already_Has_An_Ally : ArgumentException {
      public Already_Has_An_Ally(Faction faction, Faction ally) :
        base("This faction (" + faction + ") is already allied to another faction (" + ally + ")") { }
    }

    public void Ally(Faction a, Faction b);

    public class Doesn_t_Have_An_Ally : ArgumentException {
      public Doesn_t_Have_An_Ally(Faction faction) :
        base("This faction (" + faction + ") doesn't have an ally") { }
    }

    public class Are_Not_Allied : ArgumentException {
      public Are_Not_Allied(Faction a, Faction ally_of_a, Faction b, Faction ally_of_b) :
        base("These two factions (" + a + " and " + b + ") are not allied " +
          "(" + a + "'s ally: " + ally_of_a + ", " + b + "'s ally: " + ally_of_b + ")") { }
    }

    public void Break_Alliance(Faction a, Faction b);

    public void Break_Alliance(Faction faction);
  }
}
