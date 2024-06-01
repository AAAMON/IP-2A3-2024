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
      this.turn = 3;
      Storm_Sector = game.Map.Storm_Sector;
      Battle_Wheels = game.Battle_Wheels;
      this.Tleilaxu_Tanks = game.Tleilaxu_Tanks;
      Perspective_Generator = game;
      Players = game.Players;
      Init = game;
      Battle_Wheels = game.Battle_Wheels;
    }

    public override string name => "Storm";

    public override string moment { get; protected set; }

    private I_Setup_Initializers_And_Getters Init { get; }

    public Map_Resources.Map Map { get; }

    public uint Storm_Sector { get; private set; }

    private Tleilaxu_Tanks Tleilaxu_Tanks { get; }

    private uint turn;

    private I_Perspective_Generator Perspective_Generator { get; }

    private IReadOnlySet<Player> Players { get; }

    public (Battle_Wheel first, Battle_Wheel second) Battle_Wheels { get; }



    public int Calculate_Storm() {
      if(turn == 1)
      {
         return new Random().Next(18);
      }
      else
      {
        bool[] confirm = [false,false];
        int response = 0;
        Console.WriteLine(Battle_Wheels.first.Last_Player.Id + " " + Battle_Wheels.second.Last_Player.Id);
        while(!confirm[0] || !confirm[1])
        {
            Console.WriteLine("Introduceti nr de sectoare cu care sa fie mutat storm-ul (ex /1/phase_1_input/3)");
            string[] line = Console.ReadLine().Split("/");
            if(line[1] == Battle_Wheels.first.Last_Player.Id && line[2] == "phase_1_input")
            {
                int number = Int32.Parse(line[3]);
                if(number >= 0 && number <= 3 && !confirm[0])
                {
                    response += number;
                    confirm[0] = true;
                    Console.WriteLine("Succes");
                }
                else
                {
                    Console.WriteLine("Failure");
                }
            }
            else if (line[1] == Battle_Wheels.second.Last_Player.Id && line[2] == "phase_1_input")
            {
                int number = Int32.Parse(line[3]);
                if (number >= 0 && number <= 3 && !confirm[1])
                {
                    response += number;
                    confirm[1] = true;
                    Console.WriteLine("Succes");
                }
                else
                {
                    Console.WriteLine("Failure");
                }
            }
            else
            {
                Console.WriteLine("Failure");
            }
        }
        return response;
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

        moment = "before storm was calculated";
        string Get_Card = Wait_Until_Something.AwaitInput(3000).Result;
        Console.WriteLine(Get_Card);
        int sectors_to_move = Calculate_Storm();
        Console.WriteLine("Storm ul s a mutat " + sectors_to_move);

        moment = "storm was calculated";

        Move_Storm((uint)sectors_to_move);

        moment = "storm was moved";
        Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
        /*Move_Storm(sectors_to_move);*/
    }
  }
}
