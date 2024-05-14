using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public interface I_Final_Faction_Distribution {
    public Faction Faction_Of(Player player);

    public Option<Player> Player_Of(Faction faction);

    public IReadOnlySet<Faction> Factions_In_Play { get; }
  }
}
