using dune_library.Player_Resources;
using dune_library.Utils;
using LanguageExt.ClassInstances.Pred;
using LanguageExt.ClassInstances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dune_library.Map_Resources;
using dune_library.Map_Resoures;
using System.Text.Json.Serialization;
using dune_library.Decks.Treachery;
using static dune_library.Utils.Exceptions;
using dune_library.Player_Resources.Knowledge_Manager_Interfaces;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static System.Collections.Specialized.BitVector32;
using System.Security.Cryptography;
//using clientApi;

namespace dune_library.Phases
{
    internal class Set_Up : Phase
    {
        public Set_Up(I_Perspective_Generator perspective_generator,
          I_Setup_Initializers_And_Getters init,
          IReadOnlySet<Player> players,
          Either<Factions_Distribution_Manager, Final_Factions_Distribution> factions_distribution_raw,
          Option<Either<Player_Markers_Manager, Final_Player_Markers>> player_markers_raw,
          Map_Resources.Map map,
          uint Bene_Prediction,
          (Battle_Wheel A, Battle_Wheel B) Battle_Wheels,
          I_Input_Provider input_Provider,
          bool[] Factions_To_Move
        )
        {
            Perspective_Generator = perspective_generator;
            Init = init;
            Players = players;
            Factions_Distribution_Raw = factions_distribution_raw;
            Player_Markers_Raw = player_markers_raw;
            Map = map;
            this.Bene_Prediction = Bene_Prediction;
            this.Battle_Wheels = Battle_Wheels;
            this.Input_Provider = input_Provider;
            this.Factions_To_Move = Factions_To_Move;
        }
        private bool[] Factions_To_Move { get; set; } = new bool[6];

        private I_Input_Provider Input_Provider;
        public override string name => "Set-up";

        public override string moment { get; protected set; } = "";

        private I_Perspective_Generator Perspective_Generator { get; }

        private I_Setup_Initializers_And_Getters Init { get; }

        private IReadOnlySet<Faction> Factions_In_Play => Init.Factions_Distribution.Factions_In_Play;

        private Option<Either<Player_Markers_Manager, Final_Player_Markers>> Player_Markers_Raw { get; }

        private Forces Reserves => Init.Reserves;

        private I_Traitors_Initializer Traitors_Initializer => Init.Knowledge_Manager;

        private I_Spice_Manager Spice_Manager => Init.Knowledge_Manager;

        private I_Treachery_Cards_Manager Treachery_Cards_Manager => Init.Knowledge_Manager;

        private IReadOnlySet<Player> Players { get; }

        private Either<Factions_Distribution_Manager, Final_Factions_Distribution> Factions_Distribution_Raw { get; }

        private Map_Resources.Map Map { get; }

        private uint Bene_Prediction { get; set; }

        private (Battle_Wheel A, Battle_Wheel B) Battle_Wheels { get; }
        public override void Play_Out()
        {

            moment = "faction selection";
            {
                Factions_Distribution_Manager Factions_Distribution_Manager = Factions_Distribution_Raw.Left();

                //temporary assignment of the first faction from the free factions
                Players.ForEach(player => Factions_Distribution_Manager.Assign_Faction(player, Factions_Distribution_Manager.Free_Factions.First()));

                try
                {
                    Init.Init_Faction_Dependent_Objects();
                }
                catch (Faction_Selection_Ongoing)
                {
                    // keep the faction selection going
                }
            }

            moment = "player marker placement";
            {
                Player_Markers_Manager Player_Markers_Manager = Init.Player_Markers_Intermediary.Left();

                //temoporary assignment of the first player marker form the free markers
                Factions_In_Play.ForEach(faction => Player_Markers_Manager.Assign_Player_Marker(faction, Player_Markers_Manager.Free_Player_Markers.First()));

                try
                {
                    Init.Make_Player_Markers_Distribution_Final();
                }
                catch (Player_Marker_Selection_Ongoing)
                {
                    // keep the player marker selection ongoing
                }
            }
            Battle_Wheels.A.Last_Player = Players.ToList()[0];
            Battle_Wheels.B.Last_Player = Players.ToList()[1];

            Factions_In_Play.ForEach(faction => Console.WriteLine(Init.Factions_Distribution.Player_Of(faction).Id + " " + faction));

            moment = "Bene Gesserit prediction";
            Handle_Bene();
            
            moment = "traitor selection";
            Handle_Traitors();


            moment = "spice distribution";

            Factions_In_Play.ForEach(faction => {
                Spice_Manager.Add_Spice_To(faction, faction switch
                {
                    Faction.Atreides => 10,
                    Faction.Bene_Gesserit => 5,
                    Faction.Emperor => 10,
                    Faction.Fremen => 3,
                    Faction.Spacing_Guild => 5,
                    Faction.Harkonnen => 10,
                    _ => throw new Faction_Is_Not_Fully_Implemented(faction),
                });
            });

            moment = "initial faction forces placement";
            Factions_To_Move[3] = true;
            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));

            Factions_In_Play.ForEach(faction => ((Action)(faction switch
            {
                Faction.Atreides => () => Map.Arrakeen.Sections[0].Forces.Transfer_From(Faction.Atreides, Reserves, 10),
                Faction.Bene_Gesserit => () => Map.Polar_Sink.Sections[0].Forces.Transfer_From(Faction.Bene_Gesserit, Reserves, 1),
                Faction.Emperor => () => { }
                ,
                Faction.Fremen => () => { }
                ,
                Faction.Spacing_Guild => () => Map.Tuek_s_Sietch.Sections[0].Forces.Transfer_From(Faction.Spacing_Guild, Reserves, 5),
                Faction.Harkonnen => () => Map.Carthag.Sections[0].Forces.Transfer_From(Faction.Harkonnen, Reserves, 10),
                _ => throw new Faction_Is_Not_Fully_Implemented(faction),
            })).Invoke());

            if (Factions_In_Play.Contains(Faction.Fremen))
            {
                Forces To_Place_Now = new();
                To_Place_Now.Transfer_From(Faction.Fremen, Reserves, 10);

                int forces_distributed = 0;
                while (forces_distributed != 10)
                {
                    System.Console.WriteLine("Choose city for fremen and number of troops (/player4/setup/14/10)");
                    string[] line = Input_Provider.GetInputAsync().Result.Split("/");
                    if (line[1] == Init.Factions_Distribution.Player_Of(Faction.Fremen).Id && line[2] == "setup")
                    {
                        Console.WriteLine("player is correct");
                        int sectionid = 0;
                        int troop_number = 0;
                        if (Int32.TryParse(line[3], out sectionid) && Int32.TryParse(line[4], out troop_number))
                        {
                            if (sectionid >= 0 && sectionid <= 84 && troop_number <= 10 - forces_distributed && troop_number > 0)
                            {
                                Console.WriteLine("section id and troop number is correct");
                                if (sectionid == 14 || sectionid == 15 || sectionid == 67 || (sectionid >= 73 && sectionid <= 75))
                                {
                                    Map.Sections[sectionid].Forces.Transfer_From(Faction.Fremen, To_Place_Now, (uint)troop_number);
                                    forces_distributed += troop_number;
                                }
                                else
                                {
                                    Console.WriteLine("Failure");
                                }
                            }
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
            }
            
            moment = "treachery card distribution";

            Factions_In_Play.ForEach(faction => {
                if (faction == Faction.Harkonnen)
                {
                    Treachery_Cards_Manager.Give_A_Treachery_Card(faction);
                }
                Treachery_Cards_Manager.Give_A_Treachery_Card(faction);
            });

            moment = "end of set-up";
            Factions_To_Move.ForEach(faction => faction = false);
            Factions_In_Play.ForEach(faction =>
            {
                Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json");
            });


        }

        private void Handle_Traitors()
        {
            for (int i = 0; i < Factions_To_Move.Length; i++)
            {
                Factions_To_Move[i] = true;
            }
            Factions_To_Move[5] = false;

            IReadOnlyDictionary<Faction, IList<General>> traitors_dict = Generals_Manager.Random_Traitors(Factions_In_Play);
            IList<(bool, Faction)> faction_responses = new List<(bool, Faction)>();


            Factions_In_Play.ForEach(faction => {
                Traitors_Initializer.Init_Traitors(faction, traitors_dict[faction].ToList(), []);
                switch (faction)
                {
                    case Faction.Atreides:
                        faction_responses.Add((Factions_To_Move[0], faction));
                        break;
                    case Faction.Bene_Gesserit:
                        faction_responses.Add((Factions_To_Move[1], faction));
                        break;
                    case Faction.Emperor:
                        faction_responses.Add((Factions_To_Move[2], faction));
                        break;
                    case Faction.Fremen:
                        faction_responses.Add((Factions_To_Move[3], faction));
                        break;
                    case Faction.Spacing_Guild:
                        faction_responses.Add((Factions_To_Move[4], faction));
                        break;
                }
                Console.WriteLine();
                Console.WriteLine(faction);
                Console.WriteLine();
                traitors_dict[faction].ForEach(t => Console.WriteLine(t.Name));
            });

            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));

            while (faction_responses.Length() > 0)
            {
                string[] line = Input_Provider.GetInputAsync().Result.Split("/");
                bool correct = false;
                Factions_In_Play.ForEach(faction => {
                    if (line[1] == Init.Factions_Distribution.Player_Of(faction).Id && line[2] == "setup")
                    {
                        if (faction_responses.Contains((true, faction)))
                        {
                            for (int i = 0; i < traitors_dict[faction].ToList().Length(); i++)
                            {
                                if (traitors_dict[faction][i].Name == line[3].Replace("_", " "))
                                {
                                    General traitor = traitors_dict[faction][i];
                                    traitors_dict[faction].RemoveAt(i);
                                    Traitors_Initializer.Init_Traitors(faction, [traitor], traitors_dict[faction].ToList());
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
                                    break;
                                }
                            }
                        }
                    }
                });
                if (!correct)
                {
                    Console.WriteLine("Failure");
                }
            }
        }

        private void Handle_Bene()
        {
            Factions_To_Move[1] = true;
            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));

            Console.WriteLine("Bene Gesserit make a prediction: (/player2/setup/3)");
            while (Factions_To_Move[1] == true)
            {

                string[] line = Input_Provider.GetInputAsync().Result.Split("/");
                if (line[1] == Init.Factions_Distribution.Player_Of(Faction.Bene_Gesserit).Id && line[2] == "setup")
                {
                    int response = 0;
                    if (Int32.TryParse(line[3], out response))
                    {
                        if (response < 10 && response > 0)
                        {
                            Bene_Prediction = (uint)response;
                            Factions_To_Move[1] = false;
                        }
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
        }
    }
}
