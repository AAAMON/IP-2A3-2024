using System;
using System.Text;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        await new GameClient("girlboss", "password1").Run(args);
    }
}

public class GameClient
{
    public int playerID; /* 1 -> 6 */
    public string authToken; /* playerX */

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

        // Initialize Gamestate [!] only for testing purposes, the initialization is done by the server <-> game logic component
        /*
        string initializeResponse = await InitializeGamestate(authToken, "{\"key\":\"value\"}");
        Console.WriteLine($"Initialize Response: {initializeResponse}");
        */

        // Get Gamestate for a specific player
        string gamestate = await GetGamestate(authToken);
        Console.WriteLine($"Gamestate for {authToken}: {gamestate}");
        string game = await InitializeGamestate(authToken, gamestate);
        Console.WriteLine(game);
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
        var response = await client.PostAsync("http://localhost:1234/auth", requestBody);
        return await response.Content.ReadAsStringAsync();
    }

    static async Task<string> GetGamestate(string theAuthToken)
    {
        client.DefaultRequestHeaders.Remove("Authorization");
        client.DefaultRequestHeaders.Add("Authorization", theAuthToken);
        var response = await client.GetAsync($"http://localhost:1234/gamestate/{theAuthToken}");
        return await response.Content.ReadAsStringAsync();
    }

    static async Task<string> InitializeGamestate(string theAuthToken, string gamestate)
    {
        client.DefaultRequestHeaders.Remove("Authorization");
        client.DefaultRequestHeaders.Add("Authorization", theAuthToken);
        var requestBody = new StringContent(gamestate, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("http://localhost:1234/initialization", requestBody);
        return await response.Content.ReadAsStringAsync();
    }
}
