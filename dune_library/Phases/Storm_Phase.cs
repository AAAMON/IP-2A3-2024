using dune_library.Map_Resources;
using dune_library.Map_Resoures;
using dune_library.Player_Resources;
using dune_library.Utils;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Extensions = dune_library.Utils.Extensions;

namespace dune_library.Phases {
  public class Storm_Phase : Phase {
    public Storm_Phase(Game game) {
      Map = game.Map;
      this.turn = game.Round;
      Storm_Sector = game.Map.Storm_Sector;
      Battle_Wheels = game.Battle_Wheels;
      this.Tleilaxu_Tanks = game.Tleilaxu_Tanks;
    }

    public override string name => "Storm";

    public override string moment { get; protected set; }

    public Map_Resources.Map Map { get; }

    public uint Storm_Sector { get; private set; }

    private Tleilaxu_Tanks Tleilaxu_Tanks { get; }

    private uint turn;

    public (Battle_Wheel first, Battle_Wheel second) Battle_Wheels { get; }

    public int Calculate_Storm() {
      if(turn == 1)
      {
         return new Random().Next(20);
      }
      else
      {
        while(true)
        {
            Console.WriteLine("Introduceti nr de sectoare cu care sa fie mutat storm-ul");
            String response = Console.ReadLine();
            int number = Convert.ToInt32(response);
            if(number >= 0 && number <= 6)
                return Convert.ToInt32(response);
        }
      }
    }
    public void Move_Storm(uint sectors_to_move)
    {
        Extensions.Range(Storm_Sector + 1, sectors_to_move).ToList().ForEach(pos =>
            Map.Storm_Affectable[(int)pos].ForEach(section => {
                section.Forces.Remove_By_Storm(Tleilaxu_Tanks) ;
                section.Delete_Spice();
            })
        );
        Map.Move_Storm_Sector_Forward(sectors_to_move);
    }
        public override void Play_Out() {
        int sectors_to_move = Calculate_Storm();
        Move_Storm((uint)sectors_to_move);
        /*Move_Storm(sectors_to_move);*/
      }
    }
  }
