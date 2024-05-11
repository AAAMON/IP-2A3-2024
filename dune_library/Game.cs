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

namespace dune_library {
  public class Game {
    public static void Main() {
      System.Collections.Generic.HashSet<Player> players = [new("0"), new("1"), new("2"), new("3"), new("4"), new("5")];
      Game game = new(players);
      game.Play();
    }

    public Game(ISet<Player> players) {
      Players = players;
      // init players
      Faction_Manager = new(Players);
    }

    private void Play() {
        Set_up set_Up = new Set_up(this);
        set_Up.Play_Out();

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

    public Faction_Manager Faction_Manager { get; }

    public Player_Markers Player_Markers { get; private set; }

    public Alliances Alliances { get; private set; }

    public Territory_Card Last_Spice_Card { get; private set; } = new();

    public Forces Reserves { get; private set; }

    public Tleilaxu_Tanks Tleilaxu_Tanks { get; private set; } = new();

    public Special_Faction_Knowledge_Manager Special_Faction_Knowledge_Manager { get; private set; }

    #endregion

    #region Private Game Data

    internal Treachery_Deck Treachery_Deck { get; private set; } = new();

    internal Spice_Deck Spice_Deck { get; private set; } = new();

    #endregion

    public Perspective Generate_Perspective(Faction faction) {
      return new Perspective(faction, this);
    }
  }
}
