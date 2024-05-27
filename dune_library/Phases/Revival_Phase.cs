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
        }
        private I_Setup_Initializers_And_Getters Init { get; }

        private Tleilaxu_Tanks Tleilaxu_Tanks { get; }

        private IReadOnlySet<Faction> Factions_In_Play => Init.Factions_Distribution.Factions_In_Play;

        private I_Spice_Manager Spice_Manager => Init.Knowledge_Manager;

        private Forces Reserves;

        private I_Perspective_Generator Perspective_Generator { get; }

        private IReadOnlySet<Player> Players { get; }

        private Final_Factions_Distribution Factions_Distribution { get; }
        public override string name => "Revival";

        public override string moment { get; protected set; }

        public override void Play_Out()
        {
            Factions_In_Play.ForEach(faction => ((Action)(faction switch
            {
                Faction.Atreides => () =>
                {
                    Console.WriteLine("Faction Atreides type how many troops you want");
                    Revive(faction);
                }
                ,
                Faction.Bene_Gesserit => () =>
                {
                    Console.WriteLine("Faction Bene Gesserit type how many troops you want");
                    Revive(faction);
                }
                ,
                Faction.Emperor => () => {
                    Console.WriteLine("Faction Emperor type how many troops you want");
                    Revive(faction);
                }
                ,
                Faction.Fremen => () => {
                    Console.WriteLine("Faction Fremen type how many troops you want");
                    Revive(faction);
                }
                ,
                Faction.Spacing_Guild => () => { 
                    Console.WriteLine("Faction Spacing Guild type how many troops you want");
                    Revive(faction);
                }
                ,
                Faction.Harkonnen => () => {
                    Console.WriteLine("Faction Harkonnen type how many troops you want");
                    Revive(faction);
                }
                ,
                _ => throw new Faction_Is_Not_Fully_Implemented(faction),
            })).Invoke());

        }
        private void Revive(Faction faction) {
            if (!Tleilaxu_Tanks.Forces.Is_Present(faction))
            {
                Console.WriteLine("No troopes to be revived");
            }
            else
            {
                String factionInput = Console.ReadLine();
                uint troops_to_revive = Convert.ToUInt32(factionInput);
                Console.WriteLine(troops_to_revive);
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
                if (troops_to_revive > freeRevival)
                {
                    if (!ForceRevival(faction, troops_to_revive - freeRevival))
                    {
                        Console.WriteLine("You don't have enough spice to perform the force revival.");
                    }
                    else
                    {
                        Console.WriteLine("Succes");
                    }
                }
                else
                {
                    Tleilaxu_Tanks.Forces.Transfer_To(faction, Reserves, troops_to_revive);
                }
            }
            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));

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
        /*
        private void LeaderRevival(Faction faction)
        {
            var deadLeaders = player.Leaders.Where(leader => !leader.CanRevive).ToList();

            if (deadLeaders.Count == player.Leaders.Count)
            {
                foreach (var leader in player.Leaders)
                {
                    leader.CanRevive = true;
                }
            }

            var leaderToRevive = player.Leaders.FirstOrDefault(leader => leader.CanRevive == false);
            if (leaderToRevive != null)
            {
                int spiceCost = leaderToRevive.FightingStrength;

                if (spiceManager.AddSpice(player, spiceCost))
                {
                    leaderToRevive.CanRevive = true;
                }
            }
        }
        */
    }
}
