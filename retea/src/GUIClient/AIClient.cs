using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
public class Program
{
    public static async Task Main(string[] args)
    {
        await new ClientForAI("player1").Run(args);
    }
}

public class ClientForAI
{
    private static readonly HttpClient client = new HttpClient();
    private static string baseUrl = "http://localhost:8000/";
    private static string serverUrl = "http://localhost:1234/";
    private static int moveID = 1;
    private string player = "player1";
    private int playerID = 1;

    public ClientForAI(string player)
    {
        this.player = player;
        this.playerID = GetPlayerID();

    }
    public int GetPlayerID()
    {
        Regex regex = new Regex(@"\d");
        Match match = regex.Match(player);
        if (match.Success)
        {
            int digit = int.Parse(match.Value);
            return digit;
        }
        return 0;
    }
    public async Task Run(string[] args)
    {

        await test();
        while (true)
        {
            string gameStateJson = await GetGamestate(player);
            string moveJson = await GetMoveBody(gameStateJson);
            string moveEndpoint = AIMoveFormatToGUIFormat(moveJson);

            StringContent json = new StringContent("{\"isBot\": true}");
            HttpResponseMessage response = await client.PostAsync(serverUrl + moveEndpoint, json);
            // response.EnsureSuccessStatusCode();
            //await PostMove(moveJson);
        }
    }
    private string AIMoveFormatToGUIFormat(string AIjson)
    {
        // Parse the JSON input
        JObject jsonObject = JObject.Parse(AIjson);

        // Check for bad format or unknown phase
        if (jsonObject["status"] != null)
        {
            string status = jsonObject["status"].ToString();
            if (status == "bad format")
            {
                return "Error: Bad format.";
            }
            else if (status == "phase unknown")
            {
                return "Error: Phase unknown.";
            }
        }

        // Identify the action
        string action = jsonObject["action"]?.ToString();

        switch (action)
        {
            case "bid":
                int bidValue = (int)jsonObject["value"];
                return $"/{playerID}/phase_4_input/{bidValue}";

            case "pass":
                return $"/{playerID}/phase_4_input/pass";

            case "revive":
                int reviveValue = (int)jsonObject["value"];
                return $"/{playerID}/phase_5_input/{reviveValue}";

            case "shipment":
                int shipmentValue = (int)jsonObject["value"];
                int territory = (int)jsonObject["teritorry"];
                int section = (int)jsonObject["section"];
                return $"/{playerID}/phase_6_input/1/territory_{territory}/section_{section}/{shipmentValue}";

            case "movement":
                int sourceTerritory = (int)jsonObject["source_terittory"];
                int destinationTerritory = (int)jsonObject["destination_terittory"];
                int troopsMoved = (int)jsonObject["value"];
                return $"/{playerID}/phase_6_input/2/from_territory_{sourceTerritory}/from_section_{sourceTerritory}/to_territory_{destinationTerritory}/to_section_{destinationTerritory}/{troopsMoved}";

            case "battle":
                string generalName = jsonObject["used_general"].ToString();
                string attackCard = jsonObject["attack_card"].ToString();
                string defenseCard = jsonObject["defense_card"].ToString();
                int troops = (int)jsonObject["troops"];
                return $"/{playerID}/phase_7_input/section_id/{troops}/{generalName}/{attackCard}/{defenseCard}";

            default:
                return "Lista comenzi\nSetup:\nError: Unknown action.";
        }
    }

    private static async Task<string> GetGamestate(string authToken)
    {
        client.DefaultRequestHeaders.Remove("Authorization");
        client.DefaultRequestHeaders.Add("Authorization", authToken);
        var response = await client.GetAsync(serverUrl + $"gamestate/{authToken}");
        //response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private async Task<string> GetMoveBody(string gameStateJson)
    {
        var content = new StringContent(gameStateJson, Encoding.UTF8, "application/json");
        var url = baseUrl + "get-move-body";
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url),
            Content = content
        };

        var response = await client.SendAsync(request);
        //response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private async Task PostMove(string moveJson)
    {
        var content = new StringContent(moveJson, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(baseUrl + $"{player}/move?moveID={moveID}", content);
        //response.EnsureSuccessStatusCode();
        moveID++;
    }


    public async Task test()
    {
        string jsonFilePath = "player1.json"; // Replace with the path to your JSON file
        string jsonContent = File.ReadAllText(jsonFilePath);
        var httpContent = new StringContent(jsonContent);
        // Set the content type to application/json
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "player1");
        httpContent.Headers.ContentType.MediaType = "application/json";
        var response = await client.PostAsync(serverUrl + "gamestate/player1", httpContent);

        // Check if the request was successful
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("JSON data posted successfully.");
        }
        else
        {
            Console.WriteLine($"Error posting JSON data: {response.StatusCode}");
        }
    }

}
