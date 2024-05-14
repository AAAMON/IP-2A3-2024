using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public interface I_Factions_Manager {
    public bool Has_Faction(Player player);

    public Option<Faction> Faction_Of(Player player);

    public Option<Player> Player_Of(Faction faction);
  }
}
