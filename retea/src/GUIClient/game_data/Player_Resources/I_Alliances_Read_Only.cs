using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public interface I_Alliances_Read_Only {
    public Option<Faction> Ally_Of(Faction faction);
  }
}
