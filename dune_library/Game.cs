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

namespace dune_library {
  public class Game {
    public static void Main() {
      Game game = new();
      game.Play();
    }

    public Game() {
      // might need to do something here for Players
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

    public (Battle_Wheel A, Battle_Wheel B) Battle_Wheels { get; } = new();

    public IList<Player> Players { get; private set; } = [];

    #region Total Perspective

    public Map_Resources.Map Map { get; } = new();

    public int Storm_Sector => Map.Storm_Sector;

    public int Round { get; private set; } = 0;

    public Option<Phase> Phase { get; private set; } = None;

    public IDictionary<Faction, int> Assigned_Sector { get; private set; } = new Dictionary<Faction, int>();

    #endregion

    #region Private Game Data

    public IDictionary<Faction, Own_Player_Info> Player_Infos { get; private set; } = new Dictionary<Faction, Own_Player_Info>();

    public Treachery_Deck Treachery_Deck { get; private set; } = new();

    #endregion
  }
}
