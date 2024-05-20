using dune_library;
using dune_library.Player_Resources;
using LanguageExt.Pipes;
using dune_library.Phases;
using System.Net;
using System.Text;

namespace MyApp
{
    public class Program
    {
        public static async Task Main()
        {
            await new ValidaterServerApi().Run();
        }
    }

    public class ValidaterServerApi
    {
        static readonly HttpClient client = new HttpClient();

        public async Task Run()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:1235/");
            listener.Start();
            Console.WriteLine("Listening...");

            //the game starts with 6 players
            // it must validate this before sendind players in lobby to play
            Task receiveRequestsTask = ReceiveRequests(listener);
            Task initializeGameTask = InitializeGame();

            // waiting both to finish the tasks
            await Task.WhenAll(receiveRequestsTask, initializeGameTask);
        }
        private static async Task InitializeGame()
        {
            HashSet<Player> players = new HashSet<Player> { new Player("0"), new Player("1"), new Player("2"), new Player("3"), new Player("4"), new Player("5") };
            Game.Start(players);
            string relativePath = @"perspective.json";
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(basePath, relativePath);
            string[] playerTokens = { "player1", "player2", "player3", "player4", "player5", "player6" };
            foreach (string player in playerTokens)
            {
                string initializeResponse = await InitializeGamestate(player, path);
                Console.WriteLine($"Initialize Response for {player}: {initializeResponse}");
            }
        }
        private static async Task ReceiveRequests(HttpListener listener)
        {
            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                Console.WriteLine($"Received a {request.HttpMethod} request! at {request.Url.AbsolutePath}");

                if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/validatemove")
                {
                    Console.WriteLine("Processing the request to validate the move");

                    try
                    {
                        using StreamReader reader = new(request.InputStream, request.ContentEncoding);
                        string requestBody = await reader.ReadToEndAsync();
                        Console.WriteLine($"Request Body: {requestBody}");

                        // AICI SE VA TRIMITE RASPUNSUL IN FORMATUL DORIT
                        // !!!!
                        string responseJson = "{\"status\": \"success\", \"message\": \"E BINES!\"}";

                        byte[] buffer = Encoding.UTF8.GetBytes(responseJson);
                        response.ContentType = "application/json";

                        response.ContentLength64 = buffer.Length;
                        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing request: {ex.Message}");
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        byte[] buffer = Encoding.UTF8.GetBytes("{\"status\": \"error\", \"message\": \"Internal server error\"}");
                        response.ContentLength64 = buffer.Length;
                        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    finally
                    {
                        response.OutputStream.Close();
                    }
                }
            }
        }

        public static string ProcessRequest(string request)
        {
            if (request.StartsWith("/validatemove")) // POST
            {
                return "fac ceva intr-o functie"; // validare mutare
            }
            else
            {
                return "Invalid request";
            }
        }

        static async Task ForcedPost(string endpoint, string json)
        {
            using HttpClient client = new HttpClient();
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

        /*        static async Task PostJSONForPlayerI(string jsonContent, int i)
                {
                    string filePath = Path.Combine("initialization", $"player{i}.json");
                    await File.WriteAllTextAsync(filePath, jsonContent);
                    await ForcedPost($"initialization/player{i}", jsonContent);
                }*/

        static async Task<string> InitializeGamestate(string authToken, string path)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", authToken);
            string gamestate = await File.ReadAllTextAsync(path);
            var requestBody = new StringContent(gamestate, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:1234/initialization", requestBody);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
