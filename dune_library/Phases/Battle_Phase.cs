using dune_library.Decks.Treachery;
using dune_library.Map_Resources;
using dune_library.Map_Resoures;
using dune_library.Player_Resources;
using dune_library.Player_Resources.Knowledge_Manager_Interfaces;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Phases
{
    internal class Battle_Phase : Phase
    {
        public override string name => "Battle";

        public override string moment { get; protected set; } = "";

        public I_Input_Provider Input_Provider { get; set; }

        private I_Setup_Initializers_And_Getters Init { get; }

        private IReadOnlySet<Faction> Factions_In_Play => Init.Factions_Distribution.Factions_In_Play;

        private I_Treachery_Cards_Manager Treachery_Cards_Manager => Init.Knowledge_Manager;

        private Forces Reserves => Init.Reserves;

        public Map_Resources.Map Map { get; }

        public uint Storm_Position => Map.Storm_Sector;

        private Tleilaxu_Tanks Tleilaxu_Tanks { get; }

        private I_Perspective_Generator Perspective_Generator { get; }

        private IReadOnlySet<Player> Players { get; }

        public (Battle_Wheel first, Battle_Wheel second) Battle_Wheels { get; }

        public bool[] Factions_To_Move { get; }

        public Battle_Phase(Game game)
        {
            Input_Provider = game.Input_Provider;
            Init = game;
            Map = game.Map;
            Tleilaxu_Tanks = game.Tleilaxu_Tanks;
            Perspective_Generator = game;
            Players = game.Players;
            Battle_Wheels = game.Battle_Wheels;
            Factions_To_Move = game.Factions_To_Move;
        }

        public List<Faction> GetFactionOrder()
        {

            // Convert positions to a list of factions sorted by their positions
            List<Faction> sortedFactions = new List<Faction>();
            Factions_In_Play.ForEach(faction => sortedFactions.Add(faction));

            // Find the starting point based on the given number
            int startIndex = 0;
            for (int i = 0; i < sortedFactions.Count; i++)
            {
                if (Init.Player_Markers.Marker_Of(sortedFactions[i]) > Storm_Position)
                {
                    startIndex = i;
                    break;
                }
            }

            // Generate the ordered list starting from the determined position
            List<Faction> result = new List<Faction>();
            for (int i = startIndex; i < sortedFactions.Count; i++)
            {
                result.Add(sortedFactions[i]);
            }
            for (int i = 0; i < startIndex; i++)
            {
                result.Add(sortedFactions[i]);
            }

            return result;
        }
        private void next_faction(Faction faction)
        {
            switch (faction)
            {
                case Faction.Atreides:
                    Factions_To_Move[0] = false;
                    Factions_To_Move[1] = true;
                    break;
                case Faction.Bene_Gesserit:
                    Factions_To_Move[1] = false;
                    Factions_To_Move[2] = true;
                    break;
                case Faction.Emperor:
                    Factions_To_Move[2] = false;
                    Factions_To_Move[3] = true;
                    break;
                case Faction.Fremen:
                    Factions_To_Move[3] = false;
                    Factions_To_Move[4] = true;
                    break;
                case Faction.Spacing_Guild:
                    Factions_To_Move[4] = false;
                    Factions_To_Move[5] = true;
                    break;
                case Faction.Harkonnen:
                    Factions_To_Move[5] = false;
                    Factions_To_Move[0] = true;
                    break;
            }
        }
        public override void Play_Out()
        {
            moment = "battle initialization";
            for (int i = 0;i < Factions_To_Move.Length;i++) { Factions_To_Move[i] = false; }
            IList<Faction> faction_order = new List<Faction>(GetFactionOrder());
            switch (faction_order.First())
            {
                case Faction.Atreides:
                    Factions_To_Move[0] = true;
                    break;
                case Faction.Bene_Gesserit:
                    Factions_To_Move[1] = true;
                    break;
                case Faction.Emperor:
                    Factions_To_Move[2] = true;
                    break;
                case Faction.Fremen:
                    Factions_To_Move[3] = true;
                    break;
                case Faction.Spacing_Guild:
                    Factions_To_Move[4] = true;
                    break;
                case Faction.Harkonnen:
                    Factions_To_Move[5] = true;
                    break;
            }
            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
            
            Dictionary<Faction, List<Section>> faction_battles = [];

            faction_order.ForEach(faction => faction_battles.Add(faction, []));

            faction_order.ForEach(faction => {
                Map.Sections.ForEach(s => { 
                    if (s.Forces.Number_Of_Factions_Present >= 2 && s.Forces.Of(faction) > 0)
                    {
                        faction_battles[faction].Add(s);
                    }
                });
            });

            faction_order.ForEach(faction => {
                bool correct = false;
                while(!correct)
                {
                    Console.WriteLine("/player1/phase_7_input/player_id/section_id/number/general_name/treachery_card/treachery_card");
                    string[] line = Input_Provider.GetInputAsync().Result.Split("/");
                    int section_id;
                    if (line[1] == Init.Factions_Distribution.Player_Of(faction).Id && line[2] == "phase_7_input" && Int32.TryParse(line[3],out section_id))
                    {

                        if (faction_battles[faction].FirstOrDefault(s => s.Id == section_id) != null)
                        {
                            switch (line.Length) {
                                case 5:
                                    if (Handle_No_Cards(line[4], faction))
                                    {
                                        correct = true;
                                    }
                                    break;
                                case 6:
                                    if (Handle_One_Card())
                                    {
                                        correct = true;
                                    }
                                    break;
                                case 7:
                                    if (Handle_Two_Cards())
                                    {
                                        correct = true;
                                    }
                                    break;
                            }

                        }
                    }
                }
            });
        }

        public bool Handle_No_Cards(string general_name, Faction faction)
        {
            bool result = false;
            if (general_name == "Cheap_Hero")
            {
                if (Init.Knowledge_Manager.Of(faction).Treachery_Cards.ContainsKey(Treachery_Cards.Treachery_Card.Cheap_Hero))
                {
                    if (Init.Knowledge_Manager.Of(faction).Treachery_Cards[Treachery_Cards.Treachery_Card.Cheap_Hero] > 0)
                    {
                        Treachery_Cards_Manager.Remove_Treachery_Card(faction, Treachery_Cards.Treachery_Card.Cheap_Hero);

                        result = true;
                    }

                }
            }
            else
            {
                Generals_Manager.Get_Default_Generals_Of(faction).ForEach(general => {
                    if(general.Name == general_name.Replace("_"," "))
                    {
                        if(!Tleilaxu_Tanks.Revivable_Generals_Of(faction).Contains(general) && !Tleilaxu_Tanks.Non_Revivable_Generals_Of(faction).Contains(general))
                        {
                            result = true;
                        }
                    }
                });

            }
            return result;

        }
        public bool Handle_One_Card()
        {
            return false;
        }
        public bool Handle_Two_Cards()
        {
            return false;
        }
        public bool If_General()
        {
            return false;
        }
    }
}
