using dune_library.Map_Resources;
using dune_library.Player_Resources;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Phases
{
    /*internal class CharityPhase : Phase
    {
        private Game game;
        private string commandFilePath;

        public CharityPhase(Game game, string commandFilePath)
        {
            this.game = game;
            this.commandFilePath = commandFilePath;
        }

        private bool CheckCommandFile()
        {
            try
            {
                string command = File.ReadAllText(commandFilePath).Trim();
                return command == "CHOAM Charity";
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading command file: {e.Message}");
                return false;
            }
        }

        private int GetPlayerSpiceTotal(Player player)
        {
            return player.SpiceCards.Sum(card => card.Value);
        }

        private void EnsureMinimumSpice(Player player)
        {
            int spiceTotal = GetTotalSpice(player);
            int neededSpice = 2 - spiceTotal;
            for (int i = 0; i < neededSpice; i++)
            {
                player.SpiceCards.Add(new Spice_Card(1));
            }
        }

        public override void Play_Out()
        {
            if (!CheckCommandFile())
            {
                Console.WriteLine("Command is incorrect or missing, try again.");
                return;
            }

            Console.WriteLine("Command is correct, proceeding with CHAOM Charity phase.");
            foreach (var player in game.Players.Where(p => p.IsActive))
            {
                if (GetTotalSpice(player) <= 1)
                {
                    EnsureMinimumSpice(player);
                    Console.WriteLine($"{player.Name} has called out 'CHAOM Charity' and now has at least 2 spice.");
                }
            }
        }
    }*/
}