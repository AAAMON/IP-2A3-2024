using Npgsql;
using System;
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
        private static readonly string connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=Valeria";//aici sunt datele pentru conexiunea la BD-postgres

        private static int connectedUsers = 0;
        private static readonly HttpClient httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:1234/");
            listener.Start();
            Console.WriteLine("Server started. Listening on http://localhost:1234/");

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
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/validatemove")
                {
                    await HandleValidateMoveRequest(request, response);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/gamestate")
                {
                    await HandleUpdateGameStateRequest(request, response);
                }
                else if(request.Url.AbsolutePath.Contains("phase"))
                {
                    //the updates from GUI
                    HandleGUIInputRequest(context);
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
        private static async void HandleGUIInputRequest(HttpListenerContext context)
        {
            string url = context.Request.Url.AbsolutePath;
            if (url.Contains("_input") && context.Request.HttpMethod == "POST")
            {
                int phase1Input = int.Parse(url.Substring("/phase_1_input/".Length));

                using (var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    string json = await reader.ReadToEndAsync();
                    InputModel inputModel = Newtonsoft.Json.JsonConvert.DeserializeObject<InputModel>(json);

                    if (!phaseMatrix.ContainsKey(phase1Input))
                    {
                        phaseMatrix[phase1Input] = new Dictionary<int, int>();
                    }
                    phaseMatrix[phase1Input][inputModel.PlayerID] = phase1Input;
                    string jsonPhaseMat = ConvertPhaseMatrixToJson(phaseMatrix);

                    Console.WriteLine($"Updated matrix: Phase 1 Input: {phase1Input}, PlayerID: {inputModel.PlayerID}");
                }

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Success"));
                context.Response.Close();
            }
            else if (url.Contains("_info"))
            {
                string filePath = Path.Combine(url + "");
                Console.WriteLine(filePath);
                if (File.Exists(filePath))
                {
                    string phaseInput = File.ReadAllText(filePath);
                    await SendResponse(context.Response, HttpStatusCode.OK, phaseInput);
                }
                else
                {
                    await SendResponse(context.Response, HttpStatusCode.NotFound, "Phase info not found");
                }
            }
            else if (url.Contains("get_phase_info"))
            {
                HandleGetForJSONOnlyReqs(context.Request, context.Response);
            }
            else if (url.Contains("allinput"))
            {
                string jsonResponse = ConvertPhaseMatrixToJson(phaseMatrix);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);
                await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                context.Response.Close();
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Not Found"));
                context.Response.Close();
            }
        }
        public static string ConvertPhaseMatrixToJson(Dictionary<int, Dictionary<int, int>> matrix)
        {
            return JsonConvert.SerializeObject(matrix, Formatting.Indented);
        }

        static async Task HandleGetForJSONOnlyReqs(HttpListenerRequest request,
        HttpListenerResponse response)
        {
            string filePath = Path.Combine(request.Url + "");
            Console.WriteLine(filePath);
            if (File.Exists(filePath))
            {
                string inputFromFile = File.ReadAllText(filePath);
                Console.WriteLine("found: "+ inputFromFile);
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
                    "player" + userCount.ToString() + " username:" + username
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
            string username = credentials[0];
            string password = credentials[1];


            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();


                using (var cmd = new NpgsqlCommand("SELECT * FROM users WHERE username = @username AND password = @password", conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.FieldCount > 0) { conn.Close(); return true; }
                        else { conn.Close(); return false; }
                    }
                }

            }



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
