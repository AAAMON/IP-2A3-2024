using dune_library.Map_Resources;
using dune_library.Map_Resoures;
using dune_library.Player_Resources;
using dune_library.Player_Resources.Knowledge_Manager_Interfaces;
using dune_library.Utils;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static System.Collections.Specialized.BitVector32;

namespace dune_library.Phases
{
    public class ShipmentAndMovementPhase : Phase
    {
        public ShipmentAndMovementPhase(Game game
      )
        {
            Init = game;
            Map = game.Map;
            Input_Provider = game.Input_Provider;
            Storm_Position = game.Map.Storm_Sector;
            Factions_To_Move = game.Factions_To_Move;
            Perspective_Generator = game;
        }

        public override string name => "Shipment And Movement";

        public override string moment { get; protected set; } = "";

        public bool[] Factions_To_Move { get; }

        public uint Storm_Position;
        public I_Input_Provider Input_Provider { get; set; }

        private I_Setup_Initializers_And_Getters Init { get; }

        private IReadOnlySet<Faction> Factions_In_Play => Init.Factions_Distribution.Factions_In_Play;

        private Forces Reserves => Init.Reserves;

        private I_Spice_Manager Spice_Manager => Init.Knowledge_Manager;

        private Map_Resources.Map Map { get; }

        private Knowledge_Manager Faction_Knowledge => Init.Knowledge_Manager;

        private I_Perspective_Generator Perspective_Generator { get; }

        public override void Play_Out()
        {
            Console.WriteLine("Shipment and Movement Phase started.");
            moment = "Shipment and Movement started...";
            List<Faction> faction_order = GetFactionOrder();

            Console.WriteLine(faction_order);

            for(int i = 0; i < Factions_To_Move.Length; i++)
            {
                Factions_To_Move[i] = false;
            }

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
            faction_order.ForEach(faction =>
            {
                bool correct = false;
                while (!correct)
                {
                    Console.WriteLine($"{faction}: You have {Faction_Knowledge.getSpice(faction)} spice.");
                
                    Console.WriteLine("ex: /player1/phase_6_input/1/section_id/number_of_troops");
                    Console.WriteLine("ex: /player1/phase_6_input/2/from_section_id/to_section_id/number_of_troops");

                    string[] line = Input_Provider.GetInputAsync().Result.Split("/");
                    if (line[1] == Init.Factions_Distribution.Player_Of(faction).Id && line[2] == "phase_6_input")
                    {
                        if (line[3] == "1")
                        {
                            string sectionId = line[4];
                            string number_of_troops = line[5];
                            if(InsertTroops(faction, number_of_troops, sectionId))
                            {
                                if (Factions_In_Play.Contains(Faction.Bene_Gesserit))
                                {
                                    Handle_Bene();
                                }
                                correct = true;
                                next_faction(faction);
                            }
                            else if(Faction.Fremen == faction && Handle_Fremen_Shipment(sectionId,number_of_troops))
                            {
                                correct = true;
                                next_faction(faction);
                                if (Factions_In_Play.Contains(Faction.Bene_Gesserit))
                                {
                                    if (Init.Reserves.Of(Faction.Bene_Gesserit) > 0)
                                    {
                                        Map.Polar_Sink.Sections[0].Forces.Transfer_From(Faction.Bene_Gesserit, Reserves, 1);
                                    }
                                }
                            }
                        }
                        else if (line[3] == "2")
                        {
                            string fromSectionId = line[4];
                            string toSectionId = line[5];
                            string number_of_troops = line[6];
                            if (MoveTroops(faction, number_of_troops, fromSectionId, toSectionId))
                            {
                                correct = true;
                                next_faction(faction);
                            }
                        }
                        else if (line[3] == "pass") {
                            correct = true;
                            next_faction(faction);
                        }
                    }
                    
                }
                Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
            });
            Console.WriteLine("Shipment and Movement Phase ended.");
        }

        private void Handle_Bene()
        {
            bool[] previous_order = new bool[6];
            for(int i = 0; i < Factions_To_Move.Length; i++) { previous_order[i] = Factions_To_Move[i]; Factions_To_Move[i] = false;}
            Factions_To_Move[1] = true;
            moment = "waiting for bene input...";
            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
            bool correct = false;
            while (!correct)
            {
                Console.WriteLine("Do you want to add troops as Bene Gesserit?");
                Console.WriteLine("ex: /player2/phase_6_input/y");
                string[] line = Input_Provider.GetInputAsync().Result.Split("/");
                if (line[1] == Init.Factions_Distribution.Player_Of(Faction.Bene_Gesserit).Id && line[2] == "phase_6_input")
                {
                    if (line[3] == "y")
                    {
                        if (Init.Reserves.Of(Faction.Bene_Gesserit) > 0)
                        {
                            Map.Polar_Sink.Sections[0].Forces.Transfer_From(Faction.Bene_Gesserit, Reserves, 1);
                            correct = true;
                        }
                    }
                    else if (line[3] == "n")
                    {
                        correct = true;
                    }
                }
            }
            moment = "continue with Shipment and Movement Phase";
            for (int i = 0; i < Factions_To_Move.Length; i++) { Factions_To_Move[i] = previous_order[i]; }
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
        private bool InsertTroops(Faction faction, string troops, string sectionId)
        {
            int troops_to_place = 0;
            int section_id = 0;
            if(!Int32.TryParse(troops, out troops_to_place) || !Int32.TryParse(sectionId, out section_id) )
            {
                return false;
            }

            uint costPerTroop = 2;
            var section = Map.Sections.FirstOrDefault(s => s.Id == section_id);

            if( section == null )
            {
                return false;
            }

            if (section.Origin_Sector == Storm_Position)
            {
                return false;
            }

            if (section.Origin_Territory.Equals(Map.Habbanya_Sietch.Name) || section.Origin_Territory.Equals(Map.Carthag.Name) 
                || section.Origin_Territory.Equals(Map.Arrakeen.Name) || section.Origin_Territory.Equals(Map.Sietch_Tabr.Name) 
                || section.Origin_Territory.Equals(Map.Tuek_s_Sietch.Name) || faction == Faction.Spacing_Guild || faction == Init.Alliances.Ally_Of(Faction.Spacing_Guild))
            {
                costPerTroop = 1;
            }

            var totalCost = costPerTroop * troops_to_place;
            if(Spice_Manager.getSpice(faction) >= totalCost && Init.Reserves.Of(faction) >= troops_to_place)
            {
                if (section.Is_Full_Strongholds)
                {
                    Console.WriteLine("Number of factions present is " + section.Forces.Number_Of_Factions_Present + " you cant add anymore => skip");
                    return false;
                }
                else
                {
                    section.Forces.Transfer_From(faction, Reserves, (uint)troops_to_place);
                    Spice_Manager.Remove_Spice_From(faction, (uint)totalCost);
                    Console.WriteLine($"{troops} troops inserted into {section.Origin_Territory.Name}.");
                    if (Factions_In_Play.Contains(Faction.Spacing_Guild))
                    {
                        Spice_Manager.Add_Spice_To(Faction.Spacing_Guild, (uint)totalCost);
                    }
                    return true;
                }
            }
            else
            {
                Console.WriteLine("Not enough spice.");
                return false;
            }
        }

        private bool MoveTroops(Faction faction, string troops_to_place, string from_section_id, string to_section_id)
        {
            int toSectionId, fromSectionId, troops;
            if(!Int32.TryParse(troops_to_place, out troops) || !Int32.TryParse(from_section_id, out fromSectionId) || !Int32.TryParse(to_section_id, out toSectionId))
            {
                return false;
            }

            var fromSection = Map.Sections.FirstOrDefault(s => s.Id == fromSectionId);
            var toSection = Map.Sections.FirstOrDefault(s => s.Id == toSectionId);
            
            if (fromSection == null || toSection == null)
            {
                Console.WriteLine("Invalid territory name.");
                return false;
            }
            if(!Can_Move(fromSection.Origin_Territory,toSection.Origin_Territory,faction))
            {
                return false;
            }
            if (fromSection.Forces.Of(faction) < troops)
            {
                Console.WriteLine("Not enough troops.");
                return false;
            }
            
            fromSection.Forces.Transfer_To(faction, Reserves, (uint)troops);
            toSection.Forces.Transfer_From(faction, Reserves, (uint)troops);

            Console.WriteLine($"{troops} troops moved from {fromSection.Origin_Territory.Name} to {toSection.Origin_Territory.Name}.");
            return true;
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
    
        public bool Can_Move(Territory fromTerritory, Territory toTerritory, Faction faction)
        {
            bool hasOrnithopters = Map.Sections.FirstOrDefault(s => s.Forces.Of(faction) > 0 && (s.Origin_Territory.Name == "Arrakeen" || s.Origin_Territory.Name == "Carthag")) != null;
            if (hasOrnithopters)
            {
                List<Territory> neighbourTerritories = new List<Territory>();
                List<Territory> temp = new List<Territory>();
                fromTerritory.Sections.ForEach(s => neighbourTerritories.Add(s.Origin_Territory));
                temp = neighbourTerritories.Distinct().ToList();
                temp.ForEach(t => t.Sections.ForEach(s => neighbourTerritories.Add(s.Origin_Territory)));
                temp = neighbourTerritories.Distinct().ToList();
                temp.ForEach(t => t.Sections.ForEach(s => neighbourTerritories.Add(s.Origin_Territory)));
                if (neighbourTerritories.Contains(toTerritory))
                {
                    return true;
                }
            }
            else if (faction == Faction.Fremen)
            {
                List<Territory> neighbourTerritories = new List<Territory>();
                List<Territory> temp = new List<Territory>();
                fromTerritory.Sections.ForEach(s => neighbourTerritories.Add(s.Origin_Territory));
                temp = neighbourTerritories.Distinct().ToList();
                temp.ForEach(t => t.Sections.ForEach(s => neighbourTerritories.Add(s.Origin_Territory)));
                if (neighbourTerritories.Contains(toTerritory))
                {
                    return true;
                }
            }
            else
            {
                List<Territory> neighbourTerritories = new List<Territory>();
                fromTerritory.Sections.ForEach(s => neighbourTerritories.Add(s.Origin_Territory));
                if(neighbourTerritories.Distinct().ToList().Contains(toTerritory))
                {
                    return true;
                }
            }
            return true;
        }
        public bool Handle_Fremen_Shipment(string sectionId, string troops)
        {
            int troops_to_place = 0;
            int section_id = 0;
            if (!Int32.TryParse(troops, out troops_to_place) || !Int32.TryParse(sectionId, out section_id))
            {
                return false;
            }
            var section = Map.Sections.FirstOrDefault(s => s.Id == section_id);

            if (section == null)
            {
                return false;
            }

            if (section.Origin_Sector == Storm_Position)
            {
                return false;
            }

            List<Territory> neighbourTerritories = new List<Territory>();
            List<Territory> temp = new List<Territory>();
            Map.The_Greater_Flat.Sections.ForEach(s => neighbourTerritories.Add(s.Origin_Territory));
            temp = neighbourTerritories.Distinct().ToList();
            temp.ForEach(t => t.Sections.ForEach(s => neighbourTerritories.Add(s.Origin_Territory)));
            temp = neighbourTerritories.Distinct().ToList();
            temp.ForEach(t => t.Sections.ForEach(s => neighbourTerritories.Add(s.Origin_Territory)));

            if (neighbourTerritories.Contains(section.Origin_Territory))
            {
                if (Init.Reserves.Of(Faction.Fremen) >= troops_to_place)
                {
                    if (section.Is_Full_Strongholds)
                    {
                        Console.WriteLine("Number of factions present is " + section.Forces.Number_Of_Factions_Present + " you cant add anymore => skip");
                        return false;
                    }
                    else
                    {
                        section.Forces.Transfer_From(Faction.Fremen, Reserves, (uint)troops_to_place);
                        Console.WriteLine($"{troops} troops inserted into {section.Origin_Territory.Name}.");
                        return true;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
