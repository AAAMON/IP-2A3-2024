using dune_library;
using dune_library.Player_Resources;
using LanguageExt.Pipes;
using dune_library.Phases;
using System.Net;
using System.Text;

namespace dune_library.server
{
    public sealed class Initialize
    {
        static readonly HttpClient client = new HttpClient();
        public static bool canGet = false;
        public static string command = null;
        public async Task Run()
        {
            //the game starts with 6 players
            // it must validate this before sendind players in lobby to play
            _ = InitializeGame();
        }
 public static async Task InitializeGame()
    {
        string relativePath = @"player";
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string[] playerTokens = { "player1", "player2", "player3", "player4", "player5", "player6" };

        for (int i = 1; i <= 6; i++)
        {
            string playerFilePath = Path.Combine(basePath, $"{relativePath}{i}.json");
            string playerToken = playerTokens[i - 1];
            string initializeResponse = await InitializeGamestate(playerToken, playerFilePath);
            Console.WriteLine($"Initialize Response for {playerToken}: {initializeResponse}");
        }
    }

    public static async Task<string> InitializeGamestate(string authToken, string path)
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", authToken);
        client.DefaultRequestHeaders.Add("PlayerName", authToken); 

        string gamestate = await File.ReadAllTextAsync(path);
        var requestBody = new StringContent(gamestate, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"http://localhost:1234/initialization/{authToken}", requestBody);
        
        return await response.Content.ReadAsStringAsync();
    }
    }
}
