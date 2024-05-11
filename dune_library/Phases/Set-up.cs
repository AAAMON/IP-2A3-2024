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
using dune_library.Map_Resoures;

namespace dune_library.Phases {
  internal class Set_up : Phase {
    public Set_up(Game game) {
      Map = game.Map;
      Reserves = game.Reserves;
    }

/*public IList<Player> Players { get; }*/

    private Map Map { get; }

    private Forces Reserves { get; set; }

    public override void Play_Out() {

      #region faction reserve allocation
      //Fremen 10 forces in reserves
      //Atreides 10 forces in reserves
      //Harkonnen 10 forces in reserves
      //BENE GESSERIT 19 forces in reserves
      //Space Guild 15 forces in reserves
      //Emperor 20 forces in reserves

      Reserves = Forces.Initial_Reserves_From(Enum.GetValues<Faction>().ToHashSet());

      Map.Arrakeen.Sections[0].Forces.Transfer_From(Faction.Atreides, Reserves, 10);
      Map.Polar_Sink.Sections[0].Forces.Transfer_From(Faction.Bene_Gesserit, Reserves, 1);
      Map.Tuek_s_Sietch.Sections[0].Forces.Transfer_From(Faction.Spacing_Guild, Reserves, 5);
      Map.Carthag.Sections[0].Forces.Transfer_From(Faction.Harkonnen, Reserves, 10);

      Forces To_Place_Now = new();
      To_Place_Now.Transfer_From(Faction.Fremen, Reserves, 10);

      //custom for fremen, change later
      Map.Sietch_Tabr.Sections[0].Forces.Transfer_From(Faction.Atreides, To_Place_Now, 10);

      #endregion
      #region faction force allocation

       #endregion
      //Fremen 3 spice
      //Atreides 10 spice
      //Harkonnen 10 spice
      //BENE GESSERIT 5 spice
      //Space Guild 5 spice
      //Emperor 10 spice




      //faction set player marker

      //faction set Traitors

      //faction add Treachery Card

    }
  }
}
