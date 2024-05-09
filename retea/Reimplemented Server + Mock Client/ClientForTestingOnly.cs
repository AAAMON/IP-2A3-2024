using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            // Authentication
            string authToken = await AuthenticateUser("player1", "password1");
            Console.WriteLine($"Auth Token: {authToken}");

            // Initialize Gamestate
            string initializeResponse = await InitializeGamestate("player1", "{\"key\":\"value\"}");
            Console.WriteLine($"Initialize Response: {initializeResponse}");

            // Get Gamestate for a specific player
            string playerId = "player1";
            string gamestate = await GetGamestate(authToken, playerId);
            Console.WriteLine($"Gamestate for {playerId}: {gamestate}");
        }

        static async Task<string> AuthenticateUser(string username, string password)
        {
            var requestBody = new StringContent($"{username}:{password}", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await client.PostAsync("http://localhost:1234/auth", requestBody);
            return await response.Content.ReadAsStringAsync();
        }

        static async Task<string> GetGamestate(string authToken, string playerId)
        {
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", authToken);
            var response = await client.GetAsync($"http://localhost:1234/gamestate/{playerId}");
            return await response.Content.ReadAsStringAsync();
        }

        static async Task<string> InitializeGamestate(string authToken, string gamestate)
        {
            client.DefaultRequestHeaders.Add("Authorization", authToken);
            var requestBody = new StringContent(gamestate, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:1234/initialization", requestBody);
            return await response.Content.ReadAsStringAsync();
        }

    }
}