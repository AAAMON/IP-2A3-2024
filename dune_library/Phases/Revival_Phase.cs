using dune_library.Map_Resources;
using dune_library.Player_Resources;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Collections.Specialized.BitVector32;
using System.Security.Policy;
using dune_library.Player_Resources.Knowledge_Manager_Interfaces;
using System.Numerics;
using static dune_library.Utils.Exceptions;
using dune_library.Map_Resoures;
using LanguageExt;
using LanguageExt.Pipes;

namespace dune_library.Phases
{
    public class Revival_Phase : Phase
    {
        
        public Revival_Phase(I_Setup_Initializers_And_Getters init, Tleilaxu_Tanks Tleilaxu_Tanks, Game game)
        {

            this.Init = init;
            this.Tleilaxu_Tanks = Tleilaxu_Tanks;
            Players = game.Players;
            Perspective_Generator = game;
            Factions_Distribution = game.Factions_Distribution;
            Factions_To_Move = game.Factions_To_Move;
            Input_Provider = game.Input_Provider;
        }
        private I_Setup_Initializers_And_Getters Init { get; }

        private Tleilaxu_Tanks Tleilaxu_Tanks { get; }

        private IReadOnlySet<Faction> Factions_In_Play => Init.Factions_Distribution.Factions_In_Play;

        private I_Spice_Manager Spice_Manager => Init.Knowledge_Manager;

        private Forces Reserves => Init.Reserves;

        private I_Perspective_Generator Perspective_Generator { get; }

        private IReadOnlySet<Player> Players { get; }

        private Final_Factions_Distribution Factions_Distribution { get; }
        public override string name => "Revival";

        public override string moment { get; protected set; } = "";

        public bool[] Factions_To_Move { get; }

        public I_Input_Provider Input_Provider { get; set; }

        public override void Play_Out()
        {

            moment = "Waiting for Revivals...";
            for(int i = 0; i < Factions_To_Move.Length; i++)
            {
                Factions_To_Move[i] = true;
            }
            IList<Faction> faction_responses = new List<Faction>();
            Factions_In_Play.ForEach(faction_responses.Add);

            IList<Faction> can_faction_revive_generals = new List<Faction>();
            Factions_In_Play.ForEach(can_faction_revive_generals.Add);

            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));

            while (faction_responses.Length() > 0)
            {
                Console.WriteLine("Introduceti nr de trupe pt revive (ex /player1/phase_5_input/3)");
                Console.WriteLine("Introduceti nr de generali pt revive (ex /player1/phase_5_input/general_name)");
                Console.WriteLine("Pass (ex /player1/phase_5_input/pass)");

                string[] line = Input_Provider.GetInputAsync().Result.Split("/");
                bool correct = false;
                Init.Factions_Distribution.Factions_In_Play.ForEach((faction) => {
                    if (line[1] == Init.Factions_Distribution.Player_Of(faction).Id && line[2] == "phase_5_input" && faction_responses.Contains(faction))
                    {
                        if (line[3] == "pass")
                        {
                            faction_responses.Remove(faction);
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
                                case Faction.Harkonnen:
                                    Factions_To_Move[5] = false;
                                    break;
                            }
                            correct = true;
                        }
                        else
                        {
                            List<General> generals_to_revive = Tleilaxu_Tanks.Revivable_Generals_Of(faction).ToList();
                            if (!Tleilaxu_Tanks.Forces.Is_Present(faction) && generals_to_revive.Count() == 0 )
                            {
                                faction_responses.Remove(faction);
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
                                    case Faction.Harkonnen:
                                        Factions_To_Move[5] = false;
                                        break;
                                }
                                Console.WriteLine("No troopes to be revived");
                                correct = true;
                            }
                            else
                            {
                                int troops_number;
                                bool succes = Int32.TryParse(line[3], out troops_number);
                                if (succes)
                                {
                                    Console.WriteLine(troops_number);
                                    uint freeRevival = faction switch
                                    {
                                        Faction.Fremen => 3,
                                        Faction.Atreides => 2,
                                        Faction.Harkonnen => 2,
                                        Faction.Bene_Gesserit => 1,
                                        Faction.Spacing_Guild => 1,
                                        Faction.Emperor => 1,
                                        _ => 0
                                    };
                                    if (troops_number > freeRevival && troops_number < 4 && ForceRevival(faction, (uint)(troops_number - freeRevival)))
                                    {
                                        Tleilaxu_Tanks.Forces.Transfer_To(faction, Reserves, freeRevival);
                                        correct = true;
                                    }
                                    else
                                    {
                                        Tleilaxu_Tanks.Forces.Transfer_To(faction, Reserves, (uint)troops_number);
                                        correct = true;
                                    }
                                }
                                else
                                {
                                    for(int i = 0; i < generals_to_revive.Count; i++)
                                    {
                                        if (generals_to_revive[i].Name == line[3])
                                        {
                                            if(Spice_Manager.getSpice(faction) >= generals_to_revive[i].Strength && can_faction_revive_generals.Contains(faction))
                                            {
                                                can_faction_revive_generals.Remove(faction);
                                                Spice_Manager.Remove_Spice_From(faction, (uint)generals_to_revive[i].Strength);
                                                Tleilaxu_Tanks.Revive(generals_to_revive[i].Id);
                                                correct = true;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            
                        }
                    }

                });
                if (!correct)
                {
                    Console.WriteLine("Failure");
                }
                else
                {
                    Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
                }
            }

        }
        private bool ForceRevival(Faction faction, uint troops)
        {
            if(Spice_Manager.getSpice(faction) < troops * 2)
            {
                return false;
            }
            else
            {
                Spice_Manager.Remove_Spice_From(faction, troops * 2);
                Tleilaxu_Tanks.Forces.Transfer_To(faction, Reserves, troops);
                return true;
            }
        }
    }
}
