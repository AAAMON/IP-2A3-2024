using dune_library.Player_Resources;
using dune_library.Utils;
using LanguageExt.ClassInstances.Pred;
using LanguageExt.ClassInstances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dune_library.Map_Resources;

namespace dune_library.Phases {
  internal class Set_up : Phase {
    public Set_up(Game game) {
      Public_Faction_Knowledge = game.Public_Faction_Knowledge;
      Map = game.Map;
    }

    public IList<Player> Players { get; }

    public IDictionary<Faction, Public_Faction_Knowledge> Public_Faction_Knowledge { get; }

    public Map Map { get; }

    public override void Play_Out() {
      // shuffle spice and treachery deck
      /*Players[0].Faction = Faction.Atreides;
      Players[1].Faction = Faction.Bene_Gesserit;
      Players[2].Faction = Faction.Spacing_Guild;
      Players[3].Faction = Faction.Emperor;
      Players[4].Faction = Faction.Harkonnen;
      Players[5].Faction = Faction.Fremen;*/

      Public_Faction_Knowledge[Faction.Atreides] = new Public_Faction_Knowledge(Faction.Atreides, 10, 1, 10);
      Public_Faction_Knowledge[Faction.Bene_Gesserit] = new Public_Faction_Knowledge(Faction.Bene_Gesserit, 5, 4, 19);
      Public_Faction_Knowledge[Faction.Spacing_Guild] = new Public_Faction_Knowledge(Faction.Spacing_Guild, 5, 7, 15);
      Public_Faction_Knowledge[Faction.Emperor] = new Public_Faction_Knowledge(Faction.Emperor, 10, 10, 20);
      Public_Faction_Knowledge[Faction.Harkonnen] = new Public_Faction_Knowledge(Faction.Harkonnen, 10, 13, 10);
      Public_Faction_Knowledge[Faction.Fremen] = new Public_Faction_Knowledge(Faction.Fremen, 3, 16, 10);

      // bene gesserit guesses who will win

      // picking traitors

      //
/*Fremen => 10 between Sietch Tabr, False Wall West and False Wall South, 10 in reserve
Atreides => 10 in Arrakeen, 10 in reserve
Harkonnen => 10 in Carthag, 10 in reserve
Bene Gesserit => 1 in Polar Sink, 19 in reserve
Spacing Guild => 5 in Tuek’s Sietch, 15 in reserve
Emperor => 20 in reserve*/

    }
  }
}
