﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dune_library.Utils;
using dune_library.Player_Resources;
using LanguageExt.Pipes;
using dune_library.Map_Resources;

namespace dune_library.Phases
{
    public class MentatPausePhase : Phase
    {

        public MentatPausePhase(Game game)
        {
            Round = game.Round;
            Perspective_Generator = game;
            Init = game;
            Factions_To_Move = game.Factions_To_Move;
            Input_Provider = game.Input_Provider;
            Map = game.Map;
            Alliances = game.Alliances;
            Game_Winners = game.Game_Winners;
        }
        public Game_Winners Game_Winners { get; private set; }
        public Map_Resources.Map Map { get; }
        private I_Perspective_Generator Perspective_Generator { get; }
        public bool[] Factions_To_Move { get; }
        private I_Setup_Initializers_And_Getters Init { get; }

        public I_Input_Provider Input_Provider { get; set; }
        private uint Round { get; set; }

        private Alliances Alliances { get; set; }
        public override string name => "Mentat Pause";

        public override string moment { get; protected set; }

        public override void Play_Out()
        {
            CheckWinConditions();
            
            if (Game_Winners.hasWinner())
            {
                Console.WriteLine("End Game");
                moment = "End Game";
                Round = 11;
                Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
            }
            else
            {
                Console.WriteLine("waiting for players to confirm");
                moment = "waiting for players...";
                Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
                
                for(int i = 0; i < Factions_To_Move.Length; i++) { Factions_To_Move[i] = true; }
                IList<Faction> faction_responses = new List<Faction>();
                Init.Factions_Distribution.Factions_In_Play.ForEach(faction_responses.Add);

                while(faction_responses.Count > 0)
                {
                    Console.WriteLine("ex /player1/phase_9_input/pass)");
                    string[] line = Input_Provider.GetInputAsync().Result.Split("/");
                    bool correct = false;
                    Init.Factions_Distribution.Factions_In_Play.ForEach((faction) => {
                        if (line[1] == Init.Factions_Distribution.Player_Of(faction).Id && line[2] == "phase_9_input" && faction_responses.Contains(faction) && line[3] == "pass")
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
                Console.WriteLine("Go to next round");
            }
        }
        private Dictionary<Faction, List<Strongholds>> CalculateStrongholdOccupations()
        {
            var result = new Dictionary<Faction, List<Strongholds>>();
            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => result.Add(faction, new List<Strongholds>()));
            Map.Territories.ForEach(territory =>
            {
                if (territory is Strongholds)
                {
                    Init.Factions_Distribution.Factions_In_Play.ForEach(faction =>
                    {
                        if (territory.Sections[0].Forces.Of(faction) > 0)
                        {
                            result[faction].Add((Strongholds)territory);
                        }
                    });
                }
            });
            return result;
        }

        public void CheckWinConditions()
        {

            var strongholds_distribution = CalculateStrongholdOccupations();

            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => {
                int counter = strongholds_distribution[faction].Count;
                if (Alliances.Ally_Of(faction).IsSome)
                {
                    counter += strongholds_distribution[faction].Count;
                    if (counter > 3)
                    {
                        Game_Winners = new Game_Winners(faction, (Faction)Alliances.Ally_Of(faction));
                    }
                }
                else if (counter > 2)
                {
                    Game_Winners = new Game_Winners(faction);
                }
            });

        }
    }

}