using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServer
{
    class Server
    {
        private static readonly Dictionary<string, string> users = new Dictionary<string, string>
        {
            { "girlboss", "password1" },
            { "player2", "password2" },
            { "player1", "password1" }
        };

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

        static async Task HandleAuthRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string requestBody = await ReadRequestBody(request.InputStream);

            bool isAuthenticated = AuthenticateUser(requestBody);
            string username = requestBody.Split(':')[0];
            if (isAuthenticated)
            {
                int currentUsers = Interlocked.Increment(ref connectedUsers);
                if (currentUsers > 6)
                {
                    Interlocked.Decrement(ref connectedUsers);
                    await SendResponse(response, HttpStatusCode.BadRequest, "Game room is already full");
                    return;
                }
                await SendResponse(response, HttpStatusCode.OK, "player" + currentUsers.ToString());
            }
            else
            {
                await SendResponse(response, HttpStatusCode.Unauthorized, "Invalid credentials");
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

        static bool AuthenticateUser(string requestBody)
        {
            string[] credentials = requestBody.Split(':');
            string username = credentials[0];
            string password = credentials[1];

            return users.ContainsKey(username) && users[username] == password;
        }

        static bool ValidateAuthToken(string authToken)
        {
            HashSet<string> playerNames = new HashSet<string>();
            for (int i = 1; i <= 6; i++)
                playerNames.Add("player" + i.ToString());

            return !string.IsNullOrEmpty(authToken) && playerNames.Contains(authToken);
        }

        static async Task<string> ReadRequestBody(Stream inputStream)
        {
            using (var reader = new StreamReader(inputStream))
            {
                return await reader.ReadToEndAsync();
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
    }
}
