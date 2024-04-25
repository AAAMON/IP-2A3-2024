using dune_library.Map_Resources;
using dune_library.Phases;
using dune_library.Player_Resources;
using dune_library.Treachery_Cards;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library {
  internal class Game {
    public static void Main() {
      Game game = new();
      game.Play();
    }

    public Game() {
      // init Players somehow
    }

    private void Play() {
      //setup
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

    public (Battle_Wheel A, Battle_Wheel B) Battle_Wheels { get; }

    public IList<Player> Players { get; private set; }

    #region Supreme Perspective

    public Map Map { get; } = new();

    public int Storm_Sector => Map.Storm_Sector;

    public int Round { get; private set; }

    public Phase Phase { get; private set; }

    public IDictionary<Faction, int> Assigned_Sector { get; private set; }

    #endregion

    #region Private Game Data

    public IDictionary<Faction, Own_Player_Info> Player_Infos { get; private set; }

    public Treachery_Deck Treachery_Deck { get; private set; }

    #endregion
  }
}
