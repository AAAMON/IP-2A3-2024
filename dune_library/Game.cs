﻿using dune_library.Map_Resources;
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
using dune_library.Spice;
using dune_library.Map_Resoures;
using static dune_library.Utils.Exceptions;
using LanguageExt.UnsafeValueAccess;
using dune_library.Decks.Treachery;

namespace dune_library {
  public class Game : I_Faction_Dependent_Initializator_And_Getters, I_Final_Player_Markers_Distribution_Initializer, I_Perspective_Generator {
    public static void Start(IReadOnlySet<Player> players) {
      Game game = new(players);
      game.Play();
    }

    private Game(IReadOnlySet<Player> players) {
      Players = players;
      // init players
      Factions_Distribution_Manager = new(Players);
    }

    public void Play() {
        Phase = new Set_Up(this, this, Players, Factions_Distribution_Manager, Map);
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

    public IReadOnlySet<Player> Players { get; set; }

    #region Information that everyone should know

    private Factions_Distribution_Manager Factions_Distribution_Manager { get; }
    private Option<Final_Factions_Distribution> final_factions_distribution = None;
    public Final_Factions_Distribution Final_Factions_Distribution {
      get => final_factions_distribution.OrElseThrow(new Faction_Selection_Ongoing());
      private set => final_factions_distribution = value;
    }

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

    public Option<Territory_Card> Last_Spice_Card { get; private set; } = new();

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
      if (Factions_Distribution_Manager.Can_Convert_To_Final == false) {
        throw new Faction_Selection_Ongoing();
      }
      Final_Factions_Distribution = new(Players, Factions_Distribution_Manager);
      Player_Markers = new(Final_Factions_Distribution.Factions_In_Play);
      Alliances = new(Final_Factions_Distribution.Factions_In_Play);
      Reserves = Forces.Initial_Reserves_From(Final_Factions_Distribution.Factions_In_Play);
      Tleilaxu_Tanks = new(Final_Factions_Distribution.Factions_In_Play);
      Knowledge_Manager = new(Final_Factions_Distribution.Factions_In_Play, Treachery_Deck);
      faction_in_play_dependent_objects_are_initialized = true;
      return true;
    }

    #endregion

    #region Private Game Data

    internal Treachery_Deck Treachery_Deck { get; private set; } = new();

    internal Spice_Deck Spice_Deck { get; private set; } = new();

    #endregion

    public Perspective Generate_Perspective(Player player) =>
      new(
        player,
        Battle_Wheels,
        Map,
        Round,
        Phase,
        Factions_Distribution_Manager,
        final_factions_distribution,
        player_markers,
        alliances,
        reserves,
        tleilaxus_tanks,
        knowledge_manager,
        Last_Spice_Card
      );
  }
}
