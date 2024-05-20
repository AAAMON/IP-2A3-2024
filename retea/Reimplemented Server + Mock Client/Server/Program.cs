using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    class Server
    {

        private static readonly string connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=Valeria";//aici sunt datele pentru conexiunea la BD-postgres
        private static int connectedUsers = 0;

        /*  private static readonly Dictionary<string, string> authTokens = new Dictionary<string, string>(); */
        static async Task Main(string[] args)
        {
            
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
                    await HandleAuthRequest(request, response);
                }
                else if (
                    request.HttpMethod == "GET"
                    && request.Url.AbsolutePath.StartsWith("/gamestate")
                )
                {
                    await HandleGameStateGetRequest(request, response, connectedUsers);
                }
                else if (
                    request.HttpMethod == "POST"
                    && request.Url.AbsolutePath == "/initialization"
                )
                {
                    await HandleInitializationRequest(request, response);
                }
                else if (
                    request.HttpMethod == "POST"
                    && request.Url.AbsolutePath == "/validatemove"
                )
                {
                    await HandleValidateMoveRequest(request, response, connectedUsers);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/gamestate")
                {
                    await HandleUpdateGamestateRequest(request, response, connectedUsers);
                }
                else
                {
                    await SendResponse(response, HttpStatusCode.NotFound, "Not found");
                }
            }
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
                    "player" + userCount.ToString()+" username:"+username
                );
            }
            else
            {
                // Send an unauthorized response

                await SendResponse(response, HttpStatusCode.Unauthorized, "Invalid credentials" + username + parola);
            }
        }

        static async Task HandleGameStateGetRequest(
            HttpListenerRequest request,
            HttpListenerResponse response,
            int connectedUsers
        )
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

        static async Task HandleInitializationRequest(
            HttpListenerRequest request,
            HttpListenerResponse response
        )
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
                            if(reader.FieldCount > 0) { conn.Close(); return true; }
                            else { conn.Close(); return false; }
                        }
                    }

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

        static async Task<string> ReadRequestBody(Stream inputStream)
        {
            // Read the request body
            using (var reader = new StreamReader(inputStream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        static async Task SendResponse(
            HttpListenerResponse response,
            HttpStatusCode statusCode,
            string content
        )
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

            // Validate the authentication token
            string authToken = request.Headers["Authorization"];
            if (!ValidateAuthToken(authToken))
            {
                await SendResponse(response, HttpStatusCode.Unauthorized, "Invalid auth token");
                return;
            }

            // Process the move validation
            string filePath = Path.Combine("validatemove", $"{authToken}Gamestate.json");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, requestBody);
            filePath = Path.Combine("validatemove", $"{authToken}Move.json");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, requestBody);

            // Create HttpClient instance
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", authToken);

                var requestContent = new StringContent(
                    requestBody,
                    Encoding.UTF8,
                    "application/json"
                );

                // Send POST request to move validator service
                //Client for API - Game Logic
                HttpResponseMessage moveValidatorResponse = await client.PostAsync(
                    "http://localhost:1235/movevalidator",
                    requestContent
                );

                string responseContent = await moveValidatorResponse.Content.ReadAsStringAsync();
                filePath = Path.Combine("validatemove", $"{authToken}Gamestate.json");
                File.WriteAllText(filePath, responseContent);

                // Forward the response from the move validator to the GUI client
                await SendResponse(response, moveValidatorResponse.StatusCode, responseContent);
                HttpResponseMessage forwardResponse = await client.PostAsync(
                    "http://localost:1236/gameprediction",
                    new StringContent(responseContent, Encoding.UTF8, "application/json")
                );
            }
        }

        //aici fac update fara verificare de la api , desi nu inteleg de ce mai este nevoie si de
        //update daca eu trimit miscarea la validator si imi intoarce o perspectiva cu care fac
        //update
        static async Task HandleUpdateGamestateRequest(
            HttpListenerRequest request,
            HttpListenerResponse response,
            int connectedUsers
        )
        {
            string requestBody = await ReadRequestBody(request.InputStream);

            // Validate the authentication token
            string authToken = request.Headers["Authorization"];
            if (!ValidateAuthToken(authToken))
            {
                await SendResponse(response, HttpStatusCode.Unauthorized, "Invalid auth token");
                return;
            }
            string filePath = Path.Combine("gamestate", $"{authToken}.json");
            File.WriteAllText(filePath, requestBody);
            using (HttpClient client = new HttpClient())
            {
                string gamestate = File.ReadAllText(filePath);
                //forward the post to AI client
                HttpResponseMessage forwardResponse = await client.PostAsync(
                    "http://localost:1236/gameprediction",
                    new StringContent(gamestate, Encoding.UTF8, "application/json")
                );
                //then send to client -- TO DO send to all clients
                await SendResponse(response, forwardResponse.StatusCode, gamestate);
            }
        }
    }
}
