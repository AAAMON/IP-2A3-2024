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

            faction_order.ForEach(faction =>
            {
                bool correct = false;
                while (!correct)
                {
                    Console.WriteLine($"{faction}: You have {Faction_Knowledge.getSpice(faction)} spice.");
                
                    Console.WriteLine("ex: /player1/phase_6_input/1/Territory_name/section_id/number_of_troops");
                    Console.WriteLine("ex: /player1/phase_6_input/1/fromTerritory_name/section_id/toTerritory_name/section_id/number_of_troops");

                    string[] line = Input_Provider.GetInputAsync().Result.Split("/");
                    if (line[1] == Init.Factions_Distribution.Player_Of(faction).Id && line[2] == "phase_6_input")
                    {
                        if (line[3] == "1")
                        {
                            string territoryName = line[4];
                            string sectionId = line[5];
                            string number_of_troops = line[6];
                            if(InsertTroops(faction, number_of_troops, territoryName, sectionId))
                            {
                                correct = true;
                                next_faction(faction);
                                if(Factions_In_Play.Contains(Faction.Bene_Gesserit))
                                {
                                    if(Init.Reserves.Of(Faction.Bene_Gesserit) > 0)
                                    {
                                        Map.Polar_Sink.Sections[0].Forces.Transfer_From(Faction.Bene_Gesserit, Reserves, 1)
                                    }
                                }
                            }
                        }
                        else if (line[3] == "2")
                        {
                            string fromTerritoryName = line[4];
                            string fromSectionId = line[5];
                            string toTerritoryName = line[6];
                            string toSectionId = line[7];
                            string number_of_troops = line[8];
                            if (MoveTroops(faction, number_of_troops, fromTerritoryName, toTerritoryName, fromSectionId, toSectionId))
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
        private bool InsertTroops(Faction faction, string troops, string territoryName, string sectionId)
        {
            int troops_to_place = 0;
            int section_id = 0;
            if(!Int32.TryParse(troops, out troops_to_place) || !Int32.TryParse(sectionId, out section_id) )
            {
                return false;
            }

            uint costPerTroop = 2;

            var territory = Map.Territories.FirstOrDefault(t => t.Name == territoryName);
            var section = territory.Sections.FirstOrDefault(s => s.Id == section_id);

            if (territory == null || section == null)
            {
                return false;
            }

            if (territoryName.Equals(Map.Habbanya_Sietch.Name) || territoryName.Equals(Map.Carthag.Name) || territoryName.Equals(Map.Arrakeen.Name) || territoryName.Equals(Map.Sietch_Tabr.Name) || territoryName.Equals(Map.Tuek_s_Sietch.Name))
            {
                costPerTroop = 1;
            }

            var totalCost = costPerTroop * troops_to_place;

            if (Spice_Manager.Remove_Spice_From(faction, (uint)totalCost) && Init.Reserves.Of(faction) >= troops_to_place)
            {
                if (Map.Sections[(int)section.Id].Is_Full_Strongholds)
                {
                    Console.WriteLine("Number of factions present is " + territory.Sections[0].Forces.Number_Of_Factions_Present + " you cant add anymore => skip");
                    return false;
                }
                else
                {
                    Map.Sections[section_id].Forces.Transfer_From(faction, Reserves, (uint)troops_to_place);
                    Console.WriteLine($"{troops} troops inserted into {territoryName}.");
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

        private bool MoveTroops(Faction faction, string troops_to_place, string fromTerritoryName, string toTerritoryName, string from_section_id, string to_section_id)
        {
            int toSectionId, fromSectionId, troops;
            if(!Int32.TryParse(troops_to_place, out troops) || !Int32.TryParse(from_section_id, out fromSectionId) || !Int32.TryParse(to_section_id, out toSectionId))
            {
                return false;
            }

            var fromTerritory = Map.Territories.FirstOrDefault(t => t.Name == fromTerritoryName);
            var toTerritory = Map.Territories.FirstOrDefault(t => t.Name == toTerritoryName);
            var fromSection = fromTerritory.Sections.FirstOrDefault(s => s.Id == fromSectionId);
            var toSection = toTerritory.Sections.FirstOrDefault(s => s.Id == toSectionId);
            
            if (fromTerritory == null || toTerritory == null || fromSection == null || toSection == null)
            {
                Console.WriteLine("Invalid territory name.");
                return false;
            }
            if(!Can_Move(fromTerritory,toTerritory,faction))
            {
                return false;
            }
            if (fromSection.Forces.Of(faction) < troops)
            {
                Console.WriteLine("Not enough troops.");
                return false;
            }
            
            Map.Sections[(int)fromSection.Id].Forces.Transfer_To(faction, Reserves, (uint)troops);
            Map.Sections[(int)toSection.Id].Forces.Transfer_From(faction, Reserves, (uint)troops);

            Console.WriteLine($"{troops} troops moved from {fromTerritoryName} to {toTerritoryName}.");
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
    }
}
