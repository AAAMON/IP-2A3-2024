using dune_library;
using dune_library.Phases;
using dune_library.Player_Resources;
using LanguageExt.Pipes;
using System.Text;
using System.Text.Json.Nodes;

namespace MyApp
{
    internal class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            HashSet<Player> players = [new("0"), new("1"), new("2"), new("3"), new("4"), new("5")];
            Game.Start(players);

            string relativePath = @"perspective.json";
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(basePath, relativePath);
    
            string initializeResponse = await InitializeGamestate("player1", path);
            Console.WriteLine($"Initialize Response: {initializeResponse}");
            initializeResponse = await InitializeGamestate("player2", path);
            Console.WriteLine($"Initialize Response: {initializeResponse}");
            initializeResponse = await InitializeGamestate("player3", path);
            Console.WriteLine($"Initialize Response: {initializeResponse}");
            initializeResponse = await InitializeGamestate("player4", path);
            Console.WriteLine($"Initialize Response: {initializeResponse}");
            initializeResponse = await InitializeGamestate("player5", path);
            Console.WriteLine($"Initialize Response: {initializeResponse}");
            initializeResponse = await InitializeGamestate("player6", path);
            Console.WriteLine($"Initialize Response: {initializeResponse}");
        }
/*        static async void ForcedPost(string endpoint, string json)
        {
            HttpClient client = new();
            string baseUrl = "http://localhost:1234/";
            var response = await client.PostAsync($"{baseUrl}{endpoint}", new StringContent(json, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Cererea către {endpoint} a fost trimisă cu succes.");
            }
            else
            {
                Console.WriteLine($"Eroare la trimiterea cererii către {endpoint}. Codul de eroare: {response.StatusCode}");
            }
        }
        static async Task PostJSONForPlayerI(string jsonContent, int i)
        {

            string filePath = Path.Combine("initialization", $"player{i}.json");
            File.WriteAllText(filePath, jsonContent);
            ForcedPost($"initialization/player{i}", jsonContent);
        }*/
        static async Task<string> InitializeGamestate(string authToken, string path)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", authToken);
            string gamestate = File.ReadAllText(path);
            var requestBody = new StringContent(gamestate, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:1234/initialization", requestBody);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
