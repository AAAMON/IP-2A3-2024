
using MySql.Data.MySqlClient;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml;
using System.Diagnostics;
using Server;

namespace HttpServer
{
    class Server
    {
        public class InputModel
        {
            public int PlayerID { get; set; }
        }
        private static Dictionary<int, Dictionary<int, int>> phaseMatrix = new Dictionary<int, Dictionary<int, int>>();


        private static readonly ConcurrentDictionary<string, HttpListenerResponse> room =
            new ConcurrentDictionary<string, HttpListenerResponse>();

        private static int connectedUsers = 0;
        private static readonly HttpClient httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:1234/");
            listener.Start();
            Console.WriteLine("Server started. Listening on http://localhost:1234/");

            ConnectionDB db = new ConnectionDB();
            var connection = db.GetConnection(); 

            // var runner = ExeRunner.Instance;   -> FACUT PT A RULA INSTANT TOATE CELE 3 COMPONENTE

            while (true)
            {
                var context = await listener.GetContextAsync();
                _ = Task.Run(() => HandleRequest(context));
            }
        }

        static async Task HandleRequest(HttpListenerContext context)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;

                Console.WriteLine($"Received {request.HttpMethod} request for {request.Url.AbsolutePath}");

                if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/auth")
                {
                    await HandleAuthRequest(request, response);
                }
                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath.StartsWith("/gamestate"))
                {
                    await HandleGameStateGetRequest(request, response);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/initialization")
                {
                    await HandleInitializationRequest(request, response);
                }
                else if (request.Url.AbsolutePath.Contains("phase"))
                {
                    //the updates from GUI
                    HandleGUIInputRequest(context);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/gamestate")
                {
                    await HandleUpdateGameStateRequest(request, response);
                }
                else if (request.Url.AbsolutePath.Contains("/login"))
                {
                    // Extrage username și password din URL
                    string username = request.Url.Segments[2].TrimEnd('/');
                    string password = request.Url.Segments[3].TrimEnd('/'); 

                    string responseContent = HandleLoginRequest(username, password).Result;
                    await SendResponse(response, HttpStatusCode.OK, responseContent);
                }

                else
                {
                    await SendResponse(response, HttpStatusCode.NotFound, "Not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling request: {ex.Message}");
            }
        }
        public static async Task<string> HandleLoginRequest(string username, string password)
        {
            ConnectionDB db = new ConnectionDB();
            Console.WriteLine(username + " :" + password);
            bool isAuthenticated = db.PlayerExists(username, password);

            var playerData = new
            {
                username = isAuthenticated ? username : "error"
            };

            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(playerData);

            return jsonResponse;
        }

        private static async void HandleGUIInputRequest(HttpListenerContext pathRequest)
        {
            Console.WriteLine("Request for " + pathRequest);
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var requestContent = new StringContent(pathRequest.ToString(), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync($"http://localhost:1235/{pathRequest}", requestContent);

                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Request error: " + e.Message);
                }
            }
        }
        public static string ConvertPhaseMatrixToJson(Dictionary<int, Dictionary<int, int>> matrix)
        {
            return JsonConvert.SerializeObject(matrix, Newtonsoft.Json.Formatting.Indented);
        }

        static async Task HandleGetForJSONOnlyReqs(HttpListenerRequest request,
        HttpListenerResponse response)
        {
            string filePath = Path.Combine(request.Url + "");
            Console.WriteLine(filePath);
            if (File.Exists(filePath))
            {
                string inputFromFile = File.ReadAllText(filePath);
                Console.WriteLine("found: " + inputFromFile);
                await SendResponse(response, HttpStatusCode.OK, inputFromFile);
            }
            else
            {
                await SendResponse(response, HttpStatusCode.NotFound, "Phase info not found");
            }
        }

        static bool ValidateAuthToken(string authToken)
        {
            HashSet<string> playerNames = new HashSet<string>();
            for (int i = 0; i < 6; i++)
                playerNames.Add("player" + i.ToString());

            // Check if the auth token is valid
            return !string.IsNullOrEmpty(authToken) && playerNames.Contains(authToken);
        }


        static async Task HandleAuthRequest(
        HttpListenerRequest request,
        HttpListenerResponse response
)
        {
            // Read the request body containing the username and password
            string requestBody = await ReadRequestBody(request.InputStream);

            // Validate the username and password
            bool isAuthenticated = await AuthenticateUser(requestBody);
            string username = requestBody.Split(':')[0];
            string parola = requestBody.Split(":")[1];

            if (isAuthenticated)
            {
                // Increment the count of connected users atomically
                int userCount = Interlocked.Increment(ref connectedUsers);

                Console.WriteLine("Numarul de utilizatori conectati este:" + userCount);
                if (userCount > 6)
                    await SendResponse(
                        response,
                        HttpStatusCode.BadRequest,
                        "Game room is already full"
                    );
                await SendResponse(
                    response,
                    HttpStatusCode.OK,
                    "player" + connectedUsers.ToString()
                );
            }
            else
            {
                // Send an unauthorized response
                await SendResponse(response, HttpStatusCode.Unauthorized, "Invalid credentials" + username + parola);
            }
        }

        static async Task HandleGameStateGetRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string authToken = request.Headers["Authorization"];
            if (!ValidateAuthToken(authToken))
            {
                await SendResponse(response, HttpStatusCode.Unauthorized, "Invalid auth token");
                return;
            }

            if (connectedUsers != 6)
            {
                // Uncomment this line after testing
                // await SendResponse(response, HttpStatusCode.Unauthorized, "Not enough players to start the game");
                // return;
            }

            string playerId = request.Url.Segments[2].TrimEnd('/');
            string filePath = Path.Combine("gamestate", $"{playerId}.json");
            Console.WriteLine(filePath);
            if (File.Exists(filePath))
            {
                string gamestate = File.ReadAllText(filePath);
                await SendResponse(response, HttpStatusCode.OK, gamestate);
            }
            else
            {
                await SendResponse(response, HttpStatusCode.NotFound, "Gamestate not found");
            }
        }

        static async Task HandleInitializationRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string requestBody = await ReadRequestBody(request.InputStream);

            string playerName = request.Headers.Get(1);
            string filePath = Path.Combine("initialization", $"{playerName}.json");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, requestBody);
            filePath = Path.Combine("gamestate", $"{playerName}.json");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, requestBody);

            await SendResponse(response, HttpStatusCode.OK, "Initial gamestate received");
        }

        static async Task<bool> AuthenticateUser(string requestBody)
        {
            // Parse the request body to get the username and password
            string[] credentials = requestBody.Split(':');
            if (credentials.Length != 2)
            {
                return false;
            }
            string username = credentials[0];
            string password = credentials[1];

            Console.WriteLine(username + ":" + password);
            /*            using (var conn = new MySqlConnection(connectionString))
                        {
                            await conn.OpenAsync();

                            using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM users WHERE username = @username AND password = @password", conn))
                            {
                                cmd.Parameters.AddWithValue("@username", username);
                                cmd.Parameters.AddWithValue("@password", password);

                                var result = (long)await cmd.ExecuteScalarAsync();
                                return result > 0;
                            }
                        }*/
            return true;
        }

        static async Task SendResponse(HttpListenerResponse response, HttpStatusCode statusCode, string content)
        {
            response.StatusCode = (int)statusCode;
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }

        static async Task HandleValidateMoveRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string requestBody = await ReadRequestBody(request.InputStream);

            string authToken = request.Headers["Authorization"];
            if (!ValidateAuthToken(authToken))
            {
                await SendResponse(response, HttpStatusCode.Unauthorized, "Invalid auth token");
                return;
            }

            string filePath = Path.Combine("validatemove", $"{authToken}Gamestate.json");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, requestBody);
            filePath = Path.Combine("validatemove", $"{authToken}Move.json");
            File.WriteAllText(filePath, requestBody);

            var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            HttpResponseMessage moveValidatorResponse = await httpClient.PostAsync("http://localhost:1235/movevalidator", requestContent);
            string responseContent = await moveValidatorResponse.Content.ReadAsStringAsync();
            filePath = Path.Combine("validatemove", $"{authToken}Gamestate.json");
            File.WriteAllText(filePath, responseContent);

            await SendResponse(response, moveValidatorResponse.StatusCode, responseContent);
            HttpResponseMessage forwardResponse = await httpClient.PostAsync("http://localhost:1236/gameprediction", new StringContent(responseContent, Encoding.UTF8, "application/json"));
        }

        static async Task HandleUpdateGameStateRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string requestBody = await ReadRequestBody(request.InputStream);

            string authToken = request.Headers["Authorization"];
            if (!ValidateAuthToken(authToken))
            {
                await SendResponse(response, HttpStatusCode.Unauthorized, "Invalid auth token");
                return;
            }

            string filePath = Path.Combine("gamestate", $"{authToken}.json");
            File.WriteAllText(filePath, requestBody);
            string gamestate = File.ReadAllText(filePath);

            HttpResponseMessage forwardResponse = await httpClient.PostAsync("http://localhost:1236/gameprediction", new StringContent(gamestate, Encoding.UTF8, "application/json"));
            await SendResponse(response, forwardResponse.StatusCode, gamestate);
        }

        static async Task BroadcastToAllClients(string content)
        {
            foreach (var kvp in room)
            {
                try
                {
                    var response = kvp.Value;
                    await SendResponse(response, HttpStatusCode.OK, content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending to {kvp.Key}: {ex.Message}");
                }
            }
        }
        static async Task<string> ReadRequestBody(Stream inputStream)
        {
            // Read the request body
            using (var reader = new StreamReader(inputStream))
            {
                return await reader.ReadToEndAsync();
            }
        }

    }
}