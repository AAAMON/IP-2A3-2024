using dune_library.Map_Resources;
using dune_library.Player_Resources;
using dune_library.Utils;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Phases {
  public class Storm_Phase : Phase {
    public Storm_Phase(Game game, uint turn) {
      Map = game.Map;
      this.turn = turn;
      Storm_Sector = game.Map.Storm_Sector;
      Can_Play_Family_Atomics = null;
      // more complicated
      Can_Play_Weather_Control = null;
      // Players.filter(p => p.Treachery_Deck.contains(weather_control));
      // arguably the logic for this stuff should be here
      Battle_Wheels = game.Battle_Wheels;
    }

    public override string name => "Storm";

    public override string moment { get; protected set; }

    public Map_Resources.Map Map { get; }

    public uint Storm_Sector { get; private set; }

    private uint turn;
    
    public Option<Player> Can_Play_Family_Atomics { get; }

    public Option<Player> Can_Play_Weather_Control { get; }

    public (Battle_Wheel first, Battle_Wheel second) Battle_Wheels { get; }

    public int Calculate_Storm(int min, int max) {
      Console.WriteLine("Introduceti nr de sectoare cu care sa fie mutat storm-ul");
      String response = Console.ReadLine();
      return Convert.ToInt32(response);
      //return Battle_Wheel.Get_From_Range_Closed(min, max) + Battle_Wheel.Get_From_Range_Closed(min, max);
    }
    public override void Play_Out() {
      int sectors_to_move = 0;
      if (turn == 1) {
        // select the players before and after the initial storm marker
        sectors_to_move = Calculate_Storm(0, 20);
        /*Move_Storm(sectors_to_move);*/
      } else {
        sectors_to_move = Calculate_Storm(1, 3);
        // poll for the two threads, as is the case

        Map.Move_Storm_Sector_Forward((uint)sectors_to_move);
        /*Move_Storm(sectors_to_move);*/
      }
    }
  }
}
