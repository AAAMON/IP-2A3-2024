using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class TreacheryCardGUI
{
    public string Name { get; set; }
    public int Count { get; set; }
}

public class TraitorCardGUI
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Faction { get; set; }
    public int Strength { get; set; }
}

public class playerData
{
    public string faction { get; set; }
    public int spice { get; set; }
    public TreacheryCardGUI[] treacheryCards { get; set; }
    public TraitorCardGUI[] traitorCards { get; set; }
    public int forcesReserve { get; set; }
    public int forcesDead { get; set; } = 0;
    public int forcesDeployed { get; set; } = 0;
    public int newStormPosition { get; set; }
    public bool phase1NeedInput { get; set; } = true;
    public int cursedTimer { get; set; }
    public int round { get; set; }
    public int phase { get; set; } = 1;
    public int playerId { get; set; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        var server = new Server();
        await server.Start();
    }
}

public class Server
{
    private readonly ConcurrentDictionary<int, GameClient> _clients = new ConcurrentDictionary<int, GameClient>();
    private const int MaxPlayers = 6;
    private int _nextPlayerId = 1;

    public async Task Start()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:1237/");
        listener.Start();
        Console.WriteLine("Server listening...");

        while (_nextPlayerId <= MaxPlayers)
        {
            var context = await listener.GetContextAsync();
            _ = Task.Run(() => HandleClient(context));
        }
    }

    private async Task HandleClient(HttpListenerContext context)
    {
        int playerId = _nextPlayerId++;
        var gameClient = new GameClient($"player{playerId}", $"password{playerId}");
        _clients[playerId] = gameClient;

        await gameClient.Run();

        while (context.Request.IsWebSocketRequest)
        {
            var request = context.Request;
            var response = context.Response;

            string responseString = gameClient.ProcessRequest(request.RawUrl);
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            Console.WriteLine("{0} request received for {1}", request.HttpMethod, request.RawUrl);

            response.ContentType = "application/json"; // Set content type to JSON
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }

        // Remove client when done
        _clients.TryRemove(playerId, out _);
    }

    public void Broadcast(string message)
    {
        foreach (var client in _clients.Values)
        {
            client.SendMessage(message);
        }
    }
}

public class GameClient
{
    public playerData playerData1 = new playerData();
    public int playerID; /* 1 -> 6 */
    public string authToken; /* playerX */

    static readonly HttpClient client = new HttpClient();
    static string baseUrl = "http://localhost:1234/";
    private string username { get; set; }
    private string password { get; set; }

    public async Task Run()
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

        playerData1.faction = jsonObject.Faction[0];
        Console.WriteLine($"Faction: {jsonObject.Faction[0]}");
        playerData1.spice = jsonObject.Faction_Knowledge[0].Spice;
        playerData1.newStormPosition = -1;
        playerData1.cursedTimer = -1;
        playerData1.round = 1;
        playerData1.playerId = playerID;

        var treacheryCards = jsonObject.Faction_Knowledge[0].Treachery_Cards;
        int index = 0;
        playerData1.treacheryCards = new TreacheryCardGUI[5];
        foreach (var card in treacheryCards)
        {
            Console.WriteLine($"Card: {card.Name}, Quantity: {card.Value}");
            playerData1.treacheryCards[index++] = new TreacheryCardGUI { Name = card.Name, Count = card.Value };
        }

        var trtrs = jsonObject.Faction_Knowledge[0].Traitors;
        index = 0;
        playerData1.traitorCards = new TraitorCardGUI[5];
        foreach (var card in trtrs)
        {
            Console.WriteLine($"Traitor id: {card.Id}, name: {card.Name}");
            playerData1.traitorCards[index++] = new TraitorCardGUI { Id = card.Id, Name = card.Name, Faction = card.Faction, Strength = card.Strength };
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

    static async Task<string> AuthenticateUser(string username, string password)
    {
        var requestBody = new StringContent($"{username}:{password}", Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = await client.PostAsync(baseUrl + "auth", requestBody);
        return await response.Content.ReadAsStringAsync();
    }

    static async Task<string> GetGamestate(string theAuthToken)
    {
        client.DefaultRequestHeaders.Remove("Authorization");
        client.DefaultRequestHeaders.Add("Authorization", theAuthToken);
        var response = await client.GetAsync(baseUrl + $"gamestate/{theAuthToken}");
        return await response.Content.ReadAsStringAsync();
    }

    static async Task<string> InitializeGamestate(string theAuthToken, string gamestate)
    {
        client.DefaultRequestHeaders.Remove("Authorization");
        client.DefaultRequestHeaders.Add("Authorization", theAuthToken);
        var requestBody = new StringContent(gamestate, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(baseUrl + "initialization", requestBody);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> SendPhase1(int stormValue)
    {
        var response = await client.GetAsync(baseUrl + $"{playerData1.playerId}/phase_1_input/{stormValue}");
        return await response.Content.ReadAsStringAsync();
    }

    public string ProcessRequest(string request)
    {
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
        else if (request.StartsWith("/get_map_spice"))
        {
            return GetMapSpice();
        }
        else if (request.StartsWith("/get_phase_info"))
        {
            return GetPhaseInfo();
        }
        else if (request.StartsWith("/get_phase_1_info"))
        {
            return GetPhase1Info();
        }
        else if (request.StartsWith("/phase_1_input"))
        {
            int stormValue = int.Parse(request.Split('/')[2]);
            return Phase1Input(stormValue);
        }
        else
        {
            return "";
        }
    }

    public string GetPlayerData()
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(playerData1);
    }

    public string Login(string username, string password)
    {
        if (username == "girlboss" && password == "password1")
        {
            return "Valid";
        }
        else
        {
            return "Invalid";
        }
    }

    public string CheckConnection()
    {
        return "Connected";
    }

    public string GetOtherPlayersData()
    {
        return "Other Players Data";
    }

    public string GetMapSpice()
    {
        return "Map Spice";
    }

    public string GetPhaseInfo()
    {
        return "Phase Info";
    }

    public string GetPhase1Info()
    {
        return "Phase 1 Info";
    }

    public string Phase1Input(int stormValue)
    {
        return "Phase 1 Input: " + stormValue;
    }

    public void SendMessage(string message)
    {
        // Add logic to send a message to the client
        Console.WriteLine($"Message to {username}: {message}");
    }
}
