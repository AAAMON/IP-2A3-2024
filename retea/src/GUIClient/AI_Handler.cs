using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AIClient
{
    public class ClientForAI
    {
        private static readonly HttpClient client = new HttpClient();
        private static string baseUrl = "http://localhost:8080/"; 
        private static int moveID = 1;
        private string player;
        private string faction;

        public ClientForAI(string player, string faction)
        {
            this.player = player;
            this.faction = faction;
        }

        public async Task RunClientAsync(string authToken)
        {
            while (true)
            {
                string gameStateJson = await GetGamestate(authToken);
                string moveJson = await GetMoveBody(gameStateJson);
                await PostMove(moveJson);
            }
        }

        private static async Task<string> GetGamestate(string authToken)
        {
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", authToken);
            var response = await client.GetAsync(baseUrl + $"gamestate/{authToken}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> GetMoveBody(string gameStateJson)
        {
            var content = new StringContent(gameStateJson, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(baseUrl + "get-move-body", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private async Task PostMove(string moveJson)
        {
            var content = new StringContent(moveJson, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(baseUrl + $"{player}/move?moveID={moveID}", content);
            response.EnsureSuccessStatusCode();
            moveID++;
        }

        public static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: ClientForAI <playerX> <faction:[faction_type]>");
                return;
            }

            string player = args[0];
            string faction = args[1];
            string authToken = player;  // used for endpointsw

            var clientForAI = new ClientForAI(player, faction);
            await clientForAI.RunClientAsync(authToken);
        }
    }
}
