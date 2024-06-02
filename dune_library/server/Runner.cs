using dune_library.Player_Resources;
using dune_library.Utils;


namespace dune_library.server
{
    internal class Runner
    {
        public static async Task Main(string[] args)
        {
            HashSet<Player> players = [new("player1"), new("player2"), new("player3"), new("player4"), new("player5"), new("player6")];
            //await ValidaterServerApi.InitializeGame();
            //_ = new ValidaterServerApi().Run();
            Console_Input_Provider provider = new Console_Input_Provider();

            //NetworkInputProvider provider = new(1235);
            Game.Start(players, provider);
        }
    }
}
