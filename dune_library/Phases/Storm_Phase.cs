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
      Tleilaxu_Tanks = game.Tleilaxu_Tanks;
      Perspective_Generator = game;
      Players = game.Players;
      Init = game;
      Battle_Wheels = game.Battle_Wheels;
      Input_Provider = game.Input_Provider;
      Factions_To_Move = game.Factions_To_Move;
    }

    public I_Input_Provider Input_Provider { get; set; }

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

    public bool[] Factions_To_Move { get; }

    public IList<(bool, Faction)> factions_to_move()
    {
        IList<(bool, Faction)> faction_responses = new List<(bool, Faction)>();
        switch (Init.Factions_Distribution.Faction_Of(Battle_Wheels.first.Last_Player))
        {
            case Faction.Atreides:
                Factions_To_Move[0] = true;
                faction_responses.Add((true, Init.Factions_Distribution.Faction_Of(Battle_Wheels.first.Last_Player)));
                break;
            case Faction.Bene_Gesserit:
                Factions_To_Move[1] = true;
                faction_responses.Add((true, Init.Factions_Distribution.Faction_Of(Battle_Wheels.first.Last_Player)));
                break;
            case Faction.Emperor:
                Factions_To_Move[2] = true;
                faction_responses.Add((true, Init.Factions_Distribution.Faction_Of(Battle_Wheels.first.Last_Player)));
                break;
            case Faction.Fremen:
                Factions_To_Move[3] = true;
                faction_responses.Add((true, Init.Factions_Distribution.Faction_Of(Battle_Wheels.first.Last_Player)));
                break;
            case Faction.Spacing_Guild:
                Factions_To_Move[4] = true;
                faction_responses.Add((true, Init.Factions_Distribution.Faction_Of(Battle_Wheels.first.Last_Player)));
                break;
        }
        switch (Init.Factions_Distribution.Faction_Of(Battle_Wheels.second.Last_Player))
        {
            case Faction.Atreides:
                Factions_To_Move[0] = true;
                faction_responses.Add((true, Init.Factions_Distribution.Faction_Of(Battle_Wheels.second.Last_Player)));
                break;
            case Faction.Bene_Gesserit:
                Factions_To_Move[1] = true;
                faction_responses.Add((true, Init.Factions_Distribution.Faction_Of(Battle_Wheels.second.Last_Player)));
                break;
            case Faction.Emperor:
                Factions_To_Move[2] = true;
                faction_responses.Add((true, Init.Factions_Distribution.Faction_Of(Battle_Wheels.second.Last_Player)));
                break;
            case Faction.Fremen:
                Factions_To_Move[3] = true;
                faction_responses.Add((true, Init.Factions_Distribution.Faction_Of(Battle_Wheels.second.Last_Player)));
                break;
            case Faction.Spacing_Guild:
                Factions_To_Move[4] = true;
                faction_responses.Add((true, Init.Factions_Distribution.Faction_Of(Battle_Wheels.second.Last_Player)));
                break;
        }
        return faction_responses;
    }

    public int Calculate_Storm() {
      if(turn == 1)
      {
         return new Random().Next(18);
      }
      else
      {
        int response = 0;
        moment = "Calculating Storm";
        Console.WriteLine(Battle_Wheels.first.Last_Player.Id + " " + Battle_Wheels.second.Last_Player.Id);
        IList<(bool, Faction)> faction_responses = new List<(bool, Faction)>();
        faction_responses = factions_to_move();

        Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
        
        while(faction_responses.Length() > 0)
        {
            Console.WriteLine("Introduceti nr de sectoare cu care sa fie mutat storm-ul (ex /1/phase_1_input/3)");
            string[] line = Input_Provider.GetInputAsync().Result.Split("/");
            bool correct = false;
            Init.Factions_Distribution.Factions_In_Play.ForEach((faction) => { 
                if(line[1] == Init.Factions_Distribution.Player_Of(faction).Id && line[2] == "phase_1_input" && faction_responses.Contains((true, faction)))
                {
                    int number = Int32.Parse(line[3]);
                    if(number >= 0 && number <= 3)
                    {
                        response += number;
                        faction_responses.Remove((true, faction));
                        switch (faction)
                        {
                            case Faction.Atreides:
                                Factions_To_Move[0] = false;
                                break;
                            case Faction.Bene_Gesserit:
                                Factions_To_Move[1] = false;
                                break;
                            case Faction.Emperor:
                                Factions_To_Move[2] = false;
                                break;
                            case Faction.Fremen:
                                Factions_To_Move[3] = false;
                                break;
                            case Faction.Spacing_Guild:
                                Factions_To_Move[4] = false;
                                break;
                        }
                        Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
                        correct = true;
                    }
                }

            });
            if(!correct) 
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
        //string Get_Card = Wait_Until_Something.AwaitInput(3000, Input_Provider).Result;
        //Console.WriteLine(Get_Card);


        int sectors_to_move = Calculate_Storm();
        Console.WriteLine("Storm ul s a mutat " + sectors_to_move);

        moment = "storm was calculated";

        //Get_Card = Wait_Until_Something.AwaitInput(3000, Input_Provider).Result;
        //Console.WriteLine(Get_Card);

        Move_Storm((uint)sectors_to_move);
        moment = "storm was moved";

        Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
        /*Move_Storm(sectors_to_move);*/
    }
  }
}
