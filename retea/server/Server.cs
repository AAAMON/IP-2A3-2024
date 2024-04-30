using System;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace HttpServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create an HTTP listener.
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:1234/");
            listener.Start();
            Console.WriteLine("Server started. Listening on http://localhost:1234/");

            //Testing by posting Dummy JSONs
            await PostDummyJSON();

            // Handle requests asynchronously.
            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                Console.WriteLine("Catched a " + request.HttpMethod + " for a " + request.Url.Segments[1]+" and " + request.Url.Segments[2]);
                // Process GET requests
                if (request.HttpMethod == "POST")
                {
                    string[] segments = request.Url.Segments;
                    //During initialization, Game Logic API posts the initial game state of player1, player2 ... player6
                    if (segments.Length >=3 && segments[1] == "initialization/")
                    {
                        // Read JSON file.
                        string playerName = segments[2].TrimEnd('/');
                        Console.WriteLine(playerName);
                        string json = File.ReadAllText($"initialization/{playerName}.json");

                        // Forward JSON to corresponding POST request => Posts the game state of player1, player2 ... player6
                        ForcedPost($"gamestate/{playerName}", json);
                        Console.WriteLine("Posted");

                        // Send response.
                        byte[] buffer = Encoding.UTF8.GetBytes(json);
                        response.ContentLength64 = buffer.Length;
                        response.ContentType = "application/json";
                        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    }
                }
                else if (request.HttpMethod == "GET")
                {
                    /*
                     * Add creating a response +  await response.OutputStream.WriteAsync(buffer, 0, buffer.Length); <3
                     */
                }
            }
        }

        static async Task PostDummyJSON()
        {
            // Create dummy JSON files for player1 to player6
            for (int i = 1; i <= 6; i++)
            {
                string playerName = $"player{i}";
                string jsonContent = "{\"key\": \"value\"}"; // Replace this with your actual JSON content

                // Write JSON content to file
                string filePath = Path.Combine("initialization", $"{playerName}.json");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, jsonContent);
                // Post JSON content to initialization/playerX endpoint
                await ForcedPost($"initialization/{playerName}", jsonContent);
                Console.WriteLine($"Posted JSON content for {playerName} to initialization endpoint.");
            }
        }

        static async Task ForcedPost(string endpoint, string json)
        {
            HttpClient client = new HttpClient();
            string baseUrl = "http://localhost:1234/";
            client.PostAsync($"{baseUrl}{endpoint}", new StringContent(json, Encoding.UTF8, "application/json"));
        }
        static async Task ForwardToPostAsync(string endpoint, string json)
        {
            try
            {
                // Create HTTP client.
                HttpClient client = new HttpClient();
                string baseUrl = "http://localhost:1234/";
                // Make POST request to corresponding endpoint.
                HttpResponseMessage response = await client.PostAsync($"{baseUrl}{endpoint}", new StringContent(json, Encoding.UTF8, "application/json"));
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Forwarded JSON to {endpoint} successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to forward JSON to {endpoint}. Status code: {response.StatusCode}");
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response body: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

    }
}
