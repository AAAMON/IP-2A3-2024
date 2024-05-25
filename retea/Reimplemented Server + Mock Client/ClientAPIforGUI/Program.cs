using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

// aici restul de informatii


public class playerData
{
    public string faction { get; set; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        await new GameClient("girlboss", "password1").Run(args);
    }
}

public class GameClient
{
    //  for testing
    public playerData playerData1 = new playerData();
    public int playerID; /* 1 -> 6 */
    public string authToken; /* playerX */
    //public CommunicationProtocolStandards Cp;
    static readonly HttpClient client = new HttpClient();
    private readonly string baseUrl = "http://localhost:1234/";
    private string username { get; set; }
    private string password { get; set; }


    public async Task Run(string[] args)
    {
        // Authentication
        authToken = await AuthenticateUser(username, password);
        Console.WriteLine($"Auth Token: {authToken}");

        playerID = GetPlayerID();

        // Get Gamestate for a specific player
        string gamestate = await GetGamestate(authToken);
        Console.WriteLine($"Gamestate for {authToken}: {gamestate}");
        string game = await InitializeGamestate(authToken, gamestate);
        Console.WriteLine(game);

        // Deserialize the JSON string into a JObject
        dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(gamestate);

        // Access the "Faction" property
        var factionn = jsonObject.Faction;
        playerData1.faction = factionn[0];

        // Output the value of "Faction"
        Console.WriteLine($"Faction: {factionn[0]}");


        //////////////////////////////////////////////////////////////////////////////////////////
        // COMMUNICATION WITH GUI ////////////////////////////////////////////////////////////////////
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:1236/");
        listener.Start();
        Console.WriteLine("Listening...");

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string responseString = ProcessRequest(request.RawUrl);
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            Console.WriteLine("{0} request received for {1}", request.HttpMethod, request.RawUrl);


            response.ContentType = "application/json"; // Set content type to JSON
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }

    public GameClient()
    {
        InitializeDefaultCredentials();
    }
    public GameClient(string theUsername, string thePassword)
    {
        this.username = theUsername;
        this.password = thePassword;
    }

    private void InitializeDefaultCredentials()
    {
        username = "girlboss";
        password = "password1";
    }

    int GetPlayerID()
    {
        int playerID = 0;
        if (int.TryParse(authToken.Substring(6), out playerID))
        {
            Console.WriteLine("Player ID: " + playerID);
        }
        else
        {
            Console.WriteLine("Failed to extract player ID.");
        }
        return playerID;
    }

    public async Task SendPlayerAction(string action, int playerID)
    {
        string endpoint = "/playeraction/" + "player" + playerID.ToString();
        var content = new StringContent(action);
        HttpResponseMessage response = await client.PostAsync(baseUrl + endpoint, content);
        response.EnsureSuccessStatusCode();
    }

    public async Task<string> AuthenticateUser(string username, string password)
    {
        var requestBody = new StringContent($"{username}:{password}", Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = await client.PostAsync(baseUrl + "auth", requestBody);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetGamestate(string theAuthToken)
    {
        client.DefaultRequestHeaders.Remove("Authorization");
        client.DefaultRequestHeaders.Add("Authorization", theAuthToken);
        var response = await client.GetAsync(baseUrl + $"gamestate/{theAuthToken}");
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> InitializeGamestate(string theAuthToken, string gamestate)
    {
        client.DefaultRequestHeaders.Remove("Authorization");
        client.DefaultRequestHeaders.Add("Authorization", theAuthToken);
        var requestBody = new StringContent(gamestate, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(baseUrl + "initialization", requestBody);
        return await response.Content.ReadAsStringAsync();
    }
    public async Task<string> PostMoveForPlayer(string theAuthToken, string move)
        {
        client.DefaultRequestHeaders.Remove("Authorization");
        client.DefaultRequestHeaders.Add("Authorization", theAuthToken);
        var requestBody = new StringContent(move, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(baseUrl + $"/validatemove/{theAuthToken}", requestBody);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetCharittyInfo()
    {
        var response = await client.GetAsync(baseUrl+"/phase_3_info");
        return await response.Content.ReadAsStringAsync();
    }

    // STUFF FOR COMMUNICATION WITH API ////////////////////////////////////////

    public string ProcessRequest(string request)
    {
        // Parse request and call appropriate API function
        if (request.StartsWith("/get_player_data"))
        {
            return GetPlayerData();
        }
        if (request.StartsWith("/login"))
        {
            string username = request.Split('/')[2];
            string password = request.Split('/')[3];
            return Login(username, password);
        }
        else if (request.StartsWith("/connect"))
        {
            return CheckConnection();
        }
        else if (request.StartsWith("/get_other_players_data"))
        {
            return GetOtherPlayersData();
        }
        else
        {
            return "Invalid request";
        }
    }


    static string CheckConnection()
    {
        // Retrieve player data from the database or another source
        var playerData = new
        {
            message = "ok!"
        };

        // Serialize playerData object to JSON
        return Newtonsoft.Json.JsonConvert.SerializeObject(playerData);
    }

    static string Login(string username, string password)
    {
        Console.WriteLine("Trying to log in {0} with pass {1}", username, password);
        // Retrieve player data from the database or another source
        if (username == "andy" && password == "coolpassword")
        {
            var playerData = new
            {
                username = "andy"
            };
            // Serialize playerData object to JSON
            return Newtonsoft.Json.JsonConvert.SerializeObject(playerData);
        }
        else
        {
            var playerData = new
            {
                username = "error"
            };
            // Serialize playerData object to JSON
            return Newtonsoft.Json.JsonConvert.SerializeObject(playerData);
        }


    }

    public string GetPlayerData()
    {
        // Retrieve player data from the database or another source

        var playerDat = new
        {
            turnId = 1,
            faction = playerData1.faction,
            spice = 20,
            forcesReserve = 20,
            forcesDeployed = 0,
            forcesDead = 0,
            leaders = new[]
            {
                new { name = "Leader1", faction = "Atreides", power = 1, @protected = false, status = "alive" },
                new { name = "Leader2", faction = "Atreides", power = 2, @protected = false, status = "alive" },
                new { name = "Leader3", faction = "Atreides", power = 3, @protected = false, status = "alive" },
                new { name = "Leader4", faction = "Atreides", power = 4, @protected = false, status = "alive" },
                new { name = "Leader5", faction = "Atreides", power = 5, @protected = false, status = "alive" }
            },
            territories = new object[0],
            traitors = new object[0],
            treacheryCards = new object[0]
        };
        // Serialize playerData object to JSON
        return Newtonsoft.Json.JsonConvert.SerializeObject(playerDat);
    }
    static string GetOtherPlayersData()
    {
        // Retrieve player data from the database or another source
        // MAKE SURE TURN ID'S ARE IN ORDER ASCENDING !!!
        var otherPlayers = new[]
        {
            new { turnId = 2, username = "SomeDude", faction = "harkonnen", bot = "no" },
            new { turnId = 3, username = "Luffy", faction = "space_guild", bot = "no" },
            new { turnId = 4, username = "Bahhhh", faction = "bene_gesserit", bot = "no" },
            new { turnId = 5, username = "Noob420", faction = "emperor", bot = "no" },
            new { turnId = 6, username = "Bot1", faction = "fremen", bot = "yes" }
        };

        // Serialize playerData object to JSON
        return Newtonsoft.Json.JsonConvert.SerializeObject(otherPlayers);
    }
}



