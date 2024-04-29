using dune_library.Map_Resources;
using dune_library.Player_Resources;
using dune_library.Utils;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Phases {
  public class Storm_Phase : Phase {
    public Storm_Phase(Game game) {
      Map = game.Map;
      Storm_Sector = game.Map.Storm_Sector;
      Can_Play_Family_Atomics = null;
      // more complicated
      Can_Play_Weather_Control = null;
      // Players.filter(p => p.Treachery_Deck.contains(weather_control));
      // arguably the logic for this stuff should be here
      Battle_Wheels = game.Battle_Wheels;
    }

    public Map_Resources.Map Map { get; }

    public int Storm_Sector { get; private set; }

    public bool Is_First_Turn { get; }
    
    public Option<Player> Can_Play_Family_Atomics { get; }

    public Option<Player> Can_Play_Weather_Control { get; }

    public (Battle_Wheel first, Battle_Wheel second) Battle_Wheels { get; }

    public int Calculate_Storm(int min, int max) {
      return Battle_Wheel.Get_From_Range_Closed(min, max) + Battle_Wheel.Get_From_Range_Closed(min, max);
    }

    /*public void Move_Storm(int number_of_sectors) {
      // affect troops by storm
      Storm_Sector += 1;
      Enumerable.Range(Storm_Sector, number_of_sectors).ToList().ForEach(pos =>
                  Map.Storm_Affectable[pos].ForEach(section => section.Affect_By_Storm())
                );
    }*/
    public override void Play_Out() {
      int sectors_to_move = 0;
      if (Is_First_Turn) {
        // select the players before and after the initial storm marker
        sectors_to_move = Calculate_Storm(0, 20);
        /*Move_Storm(sectors_to_move);*/
      } else {
        // Thread family_atomics_asking_thread;
        // Thread weather_control_asking_thread;
        if (Can_Play_Family_Atomics.Equals(Can_Play_Family_Atomics)) { // .HasValue
          // start family_atomics_asking_thread;
        }

        sectors_to_move = Calculate_Storm(1, 3);
        if (Can_Play_Weather_Control.Equals(Can_Play_Weather_Control)) { // .HasValue
          // start weather_control_asking_thread;
        }
        
        // poll for the two threads, as is the case

        /*Move_Storm(sectors_to_move);*/
      }
    }
  }
}
