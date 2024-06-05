using dune_library;
using dune_library.Player_Resources;
using LanguageExt.Pipes;
using dune_library.Phases;
using System.Net;
using System.Text;
using System.IO;

namespace dune_library.server
{
    public sealed class GamestateSend
    {
        static readonly HttpClient client = new HttpClient();
        public static bool canGet = false;
        public static string command = null;
        public async Task Run()
        {
            _ = Sender();
        }
        public static async Task Sender()
        {
            string relativePath = @"player";
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string[] playerTokens = { "player1", "player2", "player3", "player4", "player5", "player6" };

            for (int i = 1; i <= 6; i++)
            {
                string playerFilePath = Path.Combine(basePath, $"{relativePath}{i}.json");
                string playerToken = playerTokens[i - 1];
                string initializeResponse = await POSTGamestate(playerToken, playerFilePath);
            }
        }

        public static async Task<string> POSTGamestate(string authToken, string path)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", authToken);
            client.DefaultRequestHeaders.Add("PlayerName", authToken);

            string gamestate = await File.ReadAllTextAsync(path);
            var requestBody = new StringContent(gamestate, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"http://localhost:1234/modifiedGamestate/{authToken}", requestBody);

            return await response.Content.ReadAsStringAsync();
        }

    }
}
