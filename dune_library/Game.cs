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

namespace dune_library {
  internal class Game {
    public static void Main() {
      Game game = new();
      game.Play();
    }

    public Game() {
      // might need to do something here for Players
    }

    private void Play() {
      
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

    public IList<Player> Players { get; private set; } = [];

    #region Information that everyone should know

    public (Battle_Wheel A, Battle_Wheel B) Battle_Wheels { get; } = new();

    public Map_Resources.Map Map { get; } = new();

    public int Round { get; private set; } = 0;

    public Option<Phase> Phase { get; private set; } = None;

    public Generals_Manager General_Manager { get; } = new();

    public Alliances_Manager Alliances_Manager { get; } = new();

    public Territory_Card Last_Spice_Card { get; private set; } = new();

    public IDictionary<Faction, Public_Faction_Knowledge> Public_Faction_Knowledge { get; private set; }

    public IDictionary<Faction, Special_Faction_Knowledge> Special_Faction_Knowledge { get; private set; }

    #endregion

    #region Private Game Data

    public Treachery_Deck Treachery_Deck { get; private set; } = new();

    public Spice_Deck Spice_Deck { get; private set; } = new();

    #endregion

    public Perspective Generate_Perspective(Faction faction) {
      return new Perspective(faction, this);
    }
  }
}
