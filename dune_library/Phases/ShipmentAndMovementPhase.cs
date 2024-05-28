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
          public ShipmentAndMovementPhase(
          I_Setup_Initializers_And_Getters init,
          Map_Resources.Map map
        )
            {
                Init = init;
                Map = map;
            }

            public override string name => "Shipment And Movement";

            public override string moment { get; protected set; } = "";

            private I_Setup_Initializers_And_Getters Init { get; }

            private IReadOnlySet<Faction> Factions_In_Play => Init.Factions_Distribution.Factions_In_Play;

            private Forces Reserves => Init.Reserves;

            private I_Spice_Manager Spice_Manager => Init.Knowledge_Manager;

            private Map_Resources.Map Map { get; }

            private Knowledge_Manager Faction_Knowledge => Init.Knowledge_Manager;

            public override void Play_Out()
        {
            Console.WriteLine("Shipment and Movement Phase started.");

            Factions_In_Play.ForEach(faction =>
            {
                Console.WriteLine($"{faction}: You have {Faction_Knowledge.getSpice(faction)} spice.");

                // Example: add commands to insert and move troops
                Console.WriteLine("Enter command (insert troops or move troops):");
                var command = Console.ReadLine();
                if (command.StartsWith("insert troops"))
                {
                    Console.WriteLine("ex: 6 Meridian");
                    command = Console.ReadLine();
                    var parts = command.Split(' ');
                    var troops = int.Parse(parts[0]);
                    var territoryName = parts[1];

                    InsertTroops(faction, troops, territoryName);
                }
                else if (command.StartsWith("move troops"))
                {
                    Console.WriteLine("ex: number_of_troops from_territory_name to_territory_name");
                    command = Console.ReadLine();
                    var parts = command.Split(' ');
                    var troops = int.Parse(parts[0]);
                    var fromTerritoryName = parts[1];
                    var toTerritoryName = parts[2];

                    MoveTroops(faction, troops, fromTerritoryName, toTerritoryName);
                }
            });

            Console.WriteLine("Shipment and Movement Phase ended.");
        }

        private void InsertTroops(Faction faction, int troops, string territoryName)
        {
            uint costPerTroop = 2;
            var territory = Map.Territories.FirstOrDefault(t => t.Name == territoryName);
            if (territoryName.Equals(Map.Habbanya_Sietch.Name) || territoryName.Equals(Map.Carthag.Name) || territoryName.Equals(Map.Arrakeen.Name) || territoryName.Equals(Map.Sietch_Tabr.Name) || territoryName.Equals(Map.Tuek_s_Sietch.Name))
            {
                costPerTroop = 1;
            }
            else
            {
                if (territory == null)
                {
                    Console.WriteLine("Invalid territory name.");
                    return;
                }
            }

            var totalCost = costPerTroop * (uint)troops;
            if (Spice_Manager.Remove_Spice_From(faction, totalCost))
            {
                if (territory.Sections[0].Forces.Number_Of_Factions_Present == 2)
                    Console.WriteLine("Number of factions present is " + territory.Sections[0].Forces.Number_Of_Factions_Present + " you cant add anymore => skip");
                else
                {
                    territory.Sections[0].Forces.Transfer_From(faction, Reserves, (uint)troops);
                    Console.WriteLine($"{troops} troops inserted into {territoryName}.");

                }
            }
            else
            {
                Console.WriteLine("Not enough spice.");
            }
        }

        private void MoveTroops(Faction faction, int troops, string fromTerritoryName, string toTerritoryName)
        {
            var fromTerritory = Map.Territories.FirstOrDefault(t => t.Name == fromTerritoryName);
            var toTerritory = Map.Territories.FirstOrDefault(t => t.Name == toTerritoryName);

            if (fromTerritory == null || toTerritory == null)
            {
                Console.WriteLine("Invalid territory name.");
                return;
            }
            fromTerritory.Sections[0].Forces.Transfer_To(faction, Reserves, (uint)troops);
            toTerritory.Sections[0].Forces.Transfer_From(faction, Reserves, (uint)troops);
            Console.WriteLine($"{troops} troops moved from {fromTerritoryName} to {toTerritoryName}.");
        }
    }
}
