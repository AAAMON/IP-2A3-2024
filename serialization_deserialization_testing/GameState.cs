using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dune_library;
using dune_library.Player_Resources;
using dune_library.Utils;
//using clientApi;

namespace serialization_deserialization_testing
{
    internal class GameState
    {
        public static async Task Main(string[] args)
        {
            HashSet<Player> players = [new("player1"), new("player2"), new("player3"), new("player4"), new("player5"), new("player6")];
            //await ValidaterServerApi.InitializeGame();
            //_ = new ValidaterServerApi().Run();
            Console_Input_Provider provider = new Console_Input_Provider();
            Game.Start(players, provider);
        }
    }
}
