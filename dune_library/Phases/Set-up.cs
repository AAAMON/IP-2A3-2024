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
using dune_library.Treachery_Cards;

namespace dune_library.Phases {
  internal class Set_up : Phase {
    public Set_up(Game game) {
      Game = game;
    }

    private Game Game { get; }

    private ISet<Player> Players => Game.Players;

    private Factions_Manager Factions_Manager => Game.Factions_Manager;

    private IReadOnlySet<Faction> Factions_In_Play => Factions_Manager.Factions_In_Play;

    private Player_Markers Player_Markers => Game.Player_Markers;

    private Alliances Alliances => Game.Alliances;

    private Forces Reserves => Game.Reserves;

    private Knowledge_Manager Knowledge_Manager => Game.Knowledge_Manager;

    private Map Map => Game.Map;

    private Treachery_Deck Treachery_Deck => Game.Treachery_Deck;

    //temporary
    private Faction First_Free_Faction => Factions_Manager.Free_Factions.First();

    //also temporary
    private uint First_Free_Player_Marker_position => Player_Markers.Free_Player_Markers.First();

    public override void Play_Out() {

      #region faction selection
      Players.ForEach(player => Factions_Manager.Assign_Faction(player, First_Free_Faction));

      Factions_Manager.Mark_Faction_Selection_End();
      #endregion

      Game.Init_Faction_Dependent_Objects();

      #region player marker placement
      Factions_Manager.Factions_In_Play.ForEach(faction => Player_Markers.Assign_Player_Marker(faction, First_Free_Player_Marker_position));
      #endregion

      #region traitor selection
      IReadOnlyDictionary<Faction, IList<General>> traitors_dict = Generals_Manager.Random_Traitors(Factions_In_Play);
      Factions_In_Play.ForEach(faction => {
        if (faction == Faction.Harkonnen) {
          Knowledge_Manager.Init_Traitors(faction, traitors_dict[faction].ToList(), []);
        } else {
          // this should take an imput from the user, an int from 0 to 4 exclusive
          // for now, it takes the first traitor
          var index = 0;
          General traitor = traitors_dict[faction][index];
          traitors_dict[faction].RemoveAt(index);
          Knowledge_Manager.Init_Traitors(faction, [traitor], traitors_dict[faction].ToList());
        }
      });
      #endregion

      #region spice allocation
      Knowledge_Manager.Add_Spice(Faction.Atreides, 10);
      Knowledge_Manager.Add_Spice(Faction.Bene_Gesserit, 5);
      Knowledge_Manager.Add_Spice(Faction.Emperor, 10);
      Knowledge_Manager.Add_Spice(Faction.Fremen, 3);
      Knowledge_Manager.Add_Spice(Faction.Spacing_Guild, 5);
      Knowledge_Manager.Add_Spice(Faction.Harkonnen, 10);
      #endregion

      #region faction reserve and force allocation

      Map.Arrakeen.Sections[0].Forces.Transfer_From(Faction.Atreides, Reserves, 10);
      Map.Polar_Sink.Sections[0].Forces.Transfer_From(Faction.Bene_Gesserit, Reserves, 1);
      Map.Tuek_s_Sietch.Sections[0].Forces.Transfer_From(Faction.Spacing_Guild, Reserves, 5);
      Map.Carthag.Sections[0].Forces.Transfer_From(Faction.Harkonnen, Reserves, 10);

      Forces To_Place_Now = new();
      To_Place_Now.Transfer_From(Faction.Fremen, Reserves, 10);

      //custom for fremen, change later
      Map.Sietch_Tabr.Sections[0].Forces.Transfer_From(Faction.Fremen, To_Place_Now, 10);
      #endregion

      #region temporary treachery card allocation

      Treachery_Deck.Shuffle_Deck();

      /*Factions_In_Play.ForEach(faction => {
        if (faction == Faction.Harkonnen) {
          Knowledge_Manager.Add_Treachery_Card(faction, Treachery_Deck.Pop());
        }
        Knowledge_Manager.Add_Treachery_Card(faction, Treachery_Deck.Pop());
      });*/
      #endregion



      Game.Generate_Perspective(Players.First()).SerializeToJson("perspective.json");

      //round is set to 1 in 'Game' after this

    }
  }
}
