using dune_library.Map_Resources;
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
    public static void main() {
      Game game = new();
      game.play();
    }

    public Game() {
      // init Players somehow
    }

    private void play() {
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

    public IList<Player> Players { get; private set; }

    public int Round { get; private set; }

    public int Phase { get; private set; }

    public int Moment { get; private set; }

    public Treachery_Deck Treachery_Deck { get; }

    public Map Map { get; }

    public int Storm_Sector => Map.Storm_Sector;

    public IDictionary<Faction, int> Assigned_Sector { get; }

    public IDictionary<Faction, Own_Player_Info> Player_Infos { get; }

    public (Battle_Wheel A, Battle_Wheel B) Battle_Wheels { get; }
  }
}
