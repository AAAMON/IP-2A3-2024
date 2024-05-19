using System;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HttpServer
{
    class Server
    {
        private static readonly Dictionary<string, string> users = new Dictionary<string, string>
        {
            { "girlboss", "password1" },
            { "player2", "password2" },
            {"player1","password1" }
        };

        /*  private static readonly Dictionary<string, string> authTokens = new Dictionary<string, string>(); */
        static async Task Main(string[] args)
        {
            int connectedUsers = 0;
            // Create an HTTP listener.
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:1234/");
            listener.Start();
            Console.WriteLine("Server started. Listening on http://localhost:1234/");

            // Handle requests asynchronously.
            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                Console.WriteLine($"Catched a {request.HttpMethod} for {request.Url.AbsolutePath}");

                // Process requests
                if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/auth")
                {
                    await HandleAuthRequest(request, response, connectedUsers);
                }
                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath.StartsWith("/gamestate"))
                {
                    await HandleGameStateGetRequest(request, response, connectedUsers);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/initialization")
                {
                    await HandleInitializationRequest(request, response);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/validatemove")
                {
                    await HandleValidateMoveRequest(request, response, connectedUsers);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath.StartsWith("/gamestatecalculator"))
                {
                    await HandleGameStateCalculatorRequest(request, response);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath.StartsWith("/gamestateprediction"))
                {
                    await HandleGameStatePredictionRequest(request, response);
                }

                else
                {
                    await SendResponse(response, HttpStatusCode.NotFound, "Not found");
                }
            }
        }


        static async Task HandleAuthRequest(HttpListenerRequest request, HttpListenerResponse response, int connectedUsers)
        {
            // Read the request body containing the username and password
            string requestBody = await ReadRequestBody(request.InputStream);

            // Validate the username and password
            bool isAuthenticated = AuthenticateUser(requestBody);
            string username = requestBody.Split(':')[0];
            if (isAuthenticated)
            {
                // returns auth token as player1, player2.. player6
                connectedUsers++;
                if (connectedUsers > 6)
                    await SendResponse(response, HttpStatusCode.BadRequest, "Game room is already full");
                await SendResponse(response, HttpStatusCode.OK, "player" + connectedUsers.ToString());
            }
            else
            {
                // Send an unauthorized response
                await SendResponse(response, HttpStatusCode.Unauthorized, "Invalid credentials");
            }
        }

        static async Task HandleGameStateGetRequest(HttpListenerRequest request, HttpListenerResponse response, int connectedUsers)
        {
            // Validate the authentication token
            string authToken = request.Headers["Authorization"];
            if (!ValidateAuthToken(authToken))
            {
                await SendResponse(response, HttpStatusCode.Unauthorized, "Invalid auth token");
                return;
            }
            if (connectedUsers != 6)
            {
                /* [!] the comments should be removed after testing
                   await SendResponse(response, HttpStatusCode.Unauthorized, "Not enough players to start the game");
                   return;
                */
            }

            // Get the player ID from the URL
            string playerId = request.Url.Segments[2].TrimEnd('/');

            // Handle the gamestate GET request
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
            // Read the request body containing the initial gamestate
            string requestBody = await ReadRequestBody(request.InputStream);

            // Process the initial gamestate data
            string playerName = request.Headers.Get(2);
            string filePath = Path.Combine("initialization", $"{playerName}.json");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, requestBody);
            filePath = Path.Combine("gamestate", $"{playerName}.json");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, requestBody);
            // Send a successful response
            await SendResponse(response, HttpStatusCode.OK, "Initial gamestate received");
        }

        static bool AuthenticateUser(string requestBody)
        {
            // Parse the request body to get the username and password
            string[] credentials = requestBody.Split(':');
            string username = credentials[0];
            string password = credentials[1];

            // Check if the username and password are valid
            return users.ContainsKey(username) && users[username] == password;
        }


        static bool ValidateAuthToken(string authToken)
        {
            HashSet<string> playerNames = new HashSet<string>();
            for (int i = 0; i < 6; i++)
                playerNames.Add("player" + i.ToString());

            // Check if the auth token is valid
            return !string.IsNullOrEmpty(authToken) && playerNames.Contains(authToken);
        }

        static async Task<string> ReadRequestBody(Stream inputStream)
        {
            // Read the request body
            using (var reader = new StreamReader(inputStream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        static async Task SendResponse(HttpListenerResponse response, HttpStatusCode statusCode, string content)
        {
            // Set the response status code
            response.StatusCode = (int)statusCode;

            // Set the response content
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            response.ContentLength64 = buffer.Length;

            // Send the response
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }

        static async Task HandleValidateMoveRequest(
            HttpListenerRequest request,
            HttpListenerResponse response,
            int connectedUsers
        )
        {
            string requestBody = await ReadRequestBody(request.InputStream);
            // Process the move validation
            string playerName = request.Headers["Authorization"];
            string filePath = Path.Combine("validatemove", $"{playerName}Gamestate.json");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, requestBody);
            filePath = Path.Combine("validatemove", $"{playerName}Move.json");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, requestBody);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", playerName);
                // nu sunt sigur daca ar trebui trimis la toti jucatorii sau daca se trimite tot gamestate-ul
                // ramane de gandit
                // cum am gandit eu e ca in game logic se pune valid in header daca miscarea e valida
                // si vine gamestate-ul sau ce o fi cu totul , si se trimite catre client , iar
                // in momentul in care vine asta in client care va trebui si el sa poate primi
                // request-uri (cred) , trimite post aici gamestate?turn=..\playerX 
                // de unde va fi trimis catre restul
                var valid = await client.GetAsync($"http://localhost:1235/validatemove");//portul la care ruleaza game logic pentru a valida
                string responseContent = await valid.Content.ReadAsStringAsync();
                await SendResponse(response, valid.StatusCode, responseContent);
            }
        }

        static async Task HandleGameStateCalculatorRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            // Get the player turn from the URL
            string playerTurn = request.Url.Segments[2].TrimEnd('/');

            // TODO

            // Send the updated gamestate as the response
            string updatedGamestate = ""; // Replace with the actual updated gamestate
            await SendResponse(response, HttpStatusCode.OK, updatedGamestate);
        }

        static async Task HandleGameStatePredictionRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            // Get the player turn from the URL
            string playerTurn = request.Url.Segments[2].TrimEnd('/');

            // TODO

            // Send the AI prediction as the response
            string aiPrediction = ""; // Replace with the actual AI prediction
            await SendResponse(response, HttpStatusCode.OK, aiPrediction);
        }

    }
}
