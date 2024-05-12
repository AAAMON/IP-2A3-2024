using dune_library.Map_Resources;
using dune_library.Phases;
using dune_library.Player_Resources;
using dune_library.Treachery_Cards;
using dune_library.Utils;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dune_library.Spice;
using dune_library.Map_Resoures;
using static dune_library.Utils.Exceptions;
using static dune_library.Player_Resources.Factions_Manager;
using LanguageExt.UnsafeValueAccess;

namespace dune_library {
  public class Game {
    public static void Start(ISet<Player> players) {
      Game game = new(players);
      game.Play();
    }

    private Game(ISet<Player> players) {
      Players = players;
      // init players
      Factions_Manager = new(Players);
    }

    public void Play() {
        Phase = new Set_Up(this);
        Phase.ValueUnsafe().Play_Out();

        for (Round = 1; Round <= 10; Round += 1) {
        //storm
        //spice blow
        //choam charity
        //bidding
        //revival
        //shipment and movement
        //battle
        //spice harvest
        //mentat pause
      }
    }

    public ISet<Player> Players { get; set; }

    #region Information that everyone should know

    public (Battle_Wheel A, Battle_Wheel B) Battle_Wheels { get; } = new(new(), new());

    public Map_Resources.Map Map { get; } = new();

    public uint Round { get; private set; } = 0;

    public Option<Phase> Phase { get; private set; } = None;

    private Option<Player_Markers> player_markers = None;
    public Player_Markers Player_Markers {
      get => player_markers.OrElseThrow(new Variable_Is_Not_Initialized(player_markers));
      private set => player_markers = value;
    }

    private Option<Alliances> alliances = None;
    public Alliances Alliances {
      get => alliances.OrElseThrow(new Variable_Is_Not_Initialized(alliances));
      private set => alliances = value;
    }

    public Territory_Card Last_Spice_Card { get; private set; } = new();

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

    private bool faction_in_play_dependent_objects_are_initialized;

    public bool Init_Faction_Dependent_Objects() {
      if (faction_in_play_dependent_objects_are_initialized == true) {
        return false;
      }
      IReadOnlySet<Faction> factions_in_play;
      try {
        factions_in_play = Factions_Manager.Factions_In_Play;
      } catch (Faction_Selection_Ongoing) {
        return false;
      }
      Player_Markers = new(factions_in_play);
      Alliances = new(factions_in_play);
      Reserves = Forces.Initial_Reserves_From(factions_in_play);
      Tleilaxu_Tanks = new(factions_in_play);
      Knowledge_Manager = new(factions_in_play);
      faction_in_play_dependent_objects_are_initialized = true;
      return true;
    }

    #endregion

    #region Private Game Data

    internal Factions_Manager Factions_Manager { get; }

    internal Treachery_Deck Treachery_Deck { get; private set; } = new();

    internal Spice_Deck Spice_Deck { get; private set; } = new();

    #endregion

    public Perspective Generate_Perspective(Player player) =>
      new Perspective(player, this, player_markers, alliances, reserves, tleilaxus_tanks, knowledge_manager);
  }
}
