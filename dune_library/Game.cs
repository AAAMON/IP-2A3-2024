using dune_library.Map_Resources;
using dune_library.Phases;
using dune_library.Player_Resources;
using dune_library.Utils;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dune_library.Map_Resoures;
using static dune_library.Utils.Exceptions;
using LanguageExt.UnsafeValueAccess;
using dune_library.Decks.Treachery;
using dune_library.Decks.Spice;

namespace dune_library {
  public class Game : I_Setup_Initializers_And_Getters, I_Perspective_Generator {
    public static void Start(IReadOnlySet<Player> players) {
      Game game = new(players);
      game.Play();
    }

    private Game(IReadOnlySet<Player> players) {
      Players = players;
      // init players
      factions_distribution = new Factions_Distribution_Manager(Players);
      Spice_Deck = new(Map);
    }

    public void Play() {
        Phase = new Set_Up(this, this, Players, factions_distribution, player_markers, Map);
        Phase.ValueUnsafe().Play_Out();
        for (Round = 1; Round <= 10; Round += 1) {
        //storm
        Console.WriteLine("Storm Phase");
        Phase = new Storm_Phase(this, Round);
        Phase.ValueUnsafe().Play_Out();
        //spice blow
        Console.WriteLine("Spice Blow Phase");

        //choam charity
        Console.WriteLine("Chom Charity Phase");
        Phase = new CharityPhase(this);
        Phase.ValueUnsafe().Play_Out();

        //bidding
        Console.WriteLine("Bidding Phase");

        //revival
        Console.WriteLine("Revival Phase");
        Phase = new Revival_Phase(this,Tleilaxu_Tanks);
        Phase.ValueUnsafe().Play_Out();

        //shipment and movement
        Console.WriteLine("Shipment And Movement Phase");
        Phase = new ShipmentAndMovementPhase(this, Map);
        Phase.ValueUnsafe().Play_Out();

        //battle
        Console.WriteLine("Battle Phase");


        //spice harvest
        Console.WriteLine("Spice Harvest Phase");
        Phase = new SpiceCollectionPhase(this, Map);
        Phase.ValueUnsafe().Play_Out();


        //mentat pause
        Console.WriteLine("Mentat Pause Phase");
        Phase = new MentatPausePhase(this);
        Phase.ValueUnsafe().Play_Out();
      }
    }

    public IReadOnlySet<Player> Players { get; set; }

    #region Information that everyone should know

    private Either<Factions_Distribution_Manager, Final_Factions_Distribution> factions_distribution;
    public Final_Factions_Distribution Factions_Distribution {
      get => factions_distribution.RightOrThrow(new Faction_Selection_Ongoing());
      private set => factions_distribution = value;
    }

    public (Battle_Wheel A, Battle_Wheel B) Battle_Wheels { get; } = new(new(), new());

    public Map_Resources.Map Map { get; } = new();

    public uint Round { get; private set; } = 0;

    public Option<Phase> Phase { get; private set; } = None;

    private Option<Either<Player_Markers_Manager, Final_Player_Markers>> player_markers;
    public Either<Player_Markers_Manager, Final_Player_Markers> Player_Markers_Intermediary {
      get => player_markers.OrElseThrow(new Variable_Is_Not_Initialized(player_markers));
      private set => player_markers = value;
    }
    public Final_Player_Markers Player_Markers {
      get => Player_Markers_Intermediary.RightOrThrow(new Player_Marker_Selection_Ongoing());
      private set => Player_Markers_Intermediary = value;
    }

    private Option<Alliances> alliances = None;
    public Alliances Alliances {
      get => alliances.OrElseThrow(new Variable_Is_Not_Initialized(alliances));
      private set => alliances = value;
    }

    public Option<Spice_Card> Last_Spice_Card => Spice_Deck.Top_OF_Discard_Pile;

    private Option<Forces> reserves = None;
    public Forces Reserves {
      get => reserves.OrElseThrow(new Variable_Is_Not_Initialized(reserves));
      private set => reserves = value;
    }

    private Option<Tleilaxu_Tanks> tleilaxus_tanks = None;
    public Tleilaxu_Tanks Tleilaxu_Tanks {
      get => tleilaxus_tanks.OrElseThrow(new Variable_Is_Not_Initialized(tleilaxus_tanks));
      private set => tleilaxus_tanks = value;
    }

    private Option<Knowledge_Manager> knowledge_manager = None;
    public Knowledge_Manager Knowledge_Manager {
      get => knowledge_manager.OrElseThrow(new Variable_Is_Not_Initialized(knowledge_manager));
      private set => knowledge_manager = value;
    }

    #endregion

    #region Factions in play dependent objects

    public bool Init_Faction_Dependent_Objects() {
      if (factions_distribution.IsRight) {
        return false;
      }
      if (factions_distribution.Left().Can_Convert_To_Final == false) {
        throw new Faction_Selection_Ongoing();
      }
      Factions_Distribution = new(Players, factions_distribution.Left());
      Player_Markers_Intermediary = new Player_Markers_Manager(Factions_Distribution.Factions_In_Play);
      Alliances = new(Factions_Distribution.Factions_In_Play);
      Reserves = Forces.Initial_Reserves_From(Factions_Distribution.Factions_In_Play);
      Tleilaxu_Tanks = new(Factions_Distribution.Factions_In_Play);
      Knowledge_Manager = new(Factions_Distribution.Factions_In_Play, Treachery_Deck);
      return true;
    }


    public bool Make_Player_Markers_Distribution_Final() {
      if (player_markers.IsNone) {
        throw new Faction_Selection_Ongoing();
      }
      if (player_markers.Value().IsRight) {
        return false;
      }
      if (player_markers.Value().Left().Can_Convert_To_Final == false) {
        throw new Player_Marker_Selection_Ongoing();
      }
      Player_Markers = new(Factions_Distribution.Factions_In_Play, player_markers.Value().Left());
      return true;
    }

    #endregion

    #region Private Game Data

    internal Treachery_Deck Treachery_Deck { get; } = new();

    internal Spice_Deck Spice_Deck { get; }

    #endregion

    public Perspective Generate_Perspective(Player player) =>
      new(
        player,
        Battle_Wheels,
        Map,
        Round,
        Phase,
        factions_distribution,
        player_markers,
        alliances,
        reserves,
        tleilaxus_tanks,
        knowledge_manager,
        Last_Spice_Card
      );
  }
}
