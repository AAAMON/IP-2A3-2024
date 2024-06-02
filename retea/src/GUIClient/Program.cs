
using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

// aici restul de informatii

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
public class otherPlayerGUI
{
    public int turnId { get; set; }
    public string Username { get; set; } = "Username";
    public string Faction { get; set; }
    public int Strength { get; set; }
    public int Bot { get; set; } = 0;
    public int Spice { get; set; }
    public int NrTreatcheryCards { get; set; } = 1;
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

    // OTHER PLAYERS


    public int round { get; set; }
    public int phase { get; set; } = 1;
    public int playerId { get; set; }

    // stuff for testing

    public int newStormPosition { get; set; }
    public bool phase1NeedInput { get; set; } = true;
    public int cursedTimer { get; set; }
    public bool phase4_1 { get; set; } = false;
    // put all for a phase in a class maybe
    public bool bidStopped { get; set; } = false;
    public int lastBidder { get; set; } = 6;
    public int bidValue { get; set; } = 2;
}

public class Program
{
    public static async Task Main(string[] args)
    {
        await new GameClient().Run(args);
    }
}

public class GameClient
{
    //  for testing
    public playerData playerData1 = new playerData();
    public int playerID; /* 1 -> 6 */
    public string authToken; /* playerX */

    static readonly HttpClient client = new HttpClient();
    static string baseUrl = "http://localhost:1234/";
    private string username { get; set; }
    private string password { get; set; }


    public async Task Run(string[] args)
    {
        // Authentication





        // // Get Gamestate for a specific player
        // string gamestate = await GetGamestate(authToken);
        // //Console.WriteLine($"Gamestate for {authToken}: {gamestate}");
        // string game = await InitializeGamestate(authToken, gamestate);
        // //Console.WriteLine(game);

        // // Deserialize the JSON string into a JObject
        // Console.WriteLine($"json: {gamestate}");
        // dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(gamestate);




        // //////////////////////////////////////////////////////////////////////////////////////////
        // // COMMUNICATION WITH GUI ////////////////////////////////////////////////////////////////////
        // playerData1.faction = jsonObject.Faction[0];
        // //Console.WriteLine($"Faction: {jsonObject.Faction[0]}");
        // playerData1.spice = jsonObject.Faction_Knowledge[0].Spice;
        // // playerData1.forcesReserve = jsonObject.Reserves[0]["Atreides"];
        // // if (jsonObject.Tleilaxu_Tanks[0]["Forces"] != null)
        // //     playerData1.forcesDead = jsonObject.Tleilaxu_Tanks[0]["Forces"];
        // playerData1.newStormPosition = -1;
        // playerData1.cursedTimer = -1;
        // playerData1.round = 1;
        // playerData1.playerId = playerID;
        // // treatchery cards /////////////////////////////////////////////////////////////////////////
        // var treacheryCards = jsonObject.Faction_Knowledge[0].Treachery_Cards;
        // int index = 0;
        // // Initialize the PlayerData object
        // playerData1.treacheryCards = new TreacheryCardGUI[5]; // Initialize the array with the correct size
        // foreach (var card in treacheryCards)
        // {
        //     Console.WriteLine($"Card: {card.Name}, Quantity: {card.Value}");
        //     playerData1.treacheryCards[index++] = new TreacheryCardGUI { Name = card.Name, Count = card.Value };
        // }
        // // traitors ////////////////////////////////////////////////////////////////////////////////////
        // var trtrs = jsonObject.Faction_Knowledge[0].Traitors;
        // index = 0;
        // // Initialize the PlayerData object
        // playerData1.traitorCards = new TraitorCardGUI[5]; // Initialize the array with the correct size
        // foreach (var card in trtrs)
        // {
        //     Console.WriteLine($"Traitor id: {card.Id}, name: {card.Name}");
        //     playerData1.traitorCards[index++] = new TraitorCardGUI { Id = card.Id, Name = card.Name, Faction = card.Faction, Strength = card.Strength };
        // }


        ////

        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:1236/");
        listener.Start();
        Console.WriteLine("Listening on port 1236...");

        // // Get Gamestate for a specific player
        // gamestate = await GetGamestate(authToken);
        // Console.WriteLine($"Got gamestate for {authToken}");

        // // Deserialize the JSON string into a JObject
        // jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(gamestate);
        WebSocketManager wsManager = new WebSocketManager();
        // do NOT put await on this, the code won't work, tyy <3
        wsManager.StartWebSocketServerAsync("http://localhost:2000/");

        // _ = Task.Run(async () =>
        // {

        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string responseString = await ProcessRequestAsync(request.RawUrl);
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            Console.WriteLine("{0} request received for {1}", request.HttpMethod, request.RawUrl);


            response.ContentType = "application/json"; // Set content type to JSON
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
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

    // Andreea: idk cat de bun e
    public async Task<string> SendPhase1(int stormValue)
    {
        var response = await client.GetAsync(baseUrl + $"{playerData1.playerId}/phase_1_input/{stormValue}");
        return await response.Content.ReadAsStringAsync();
    }
    public async Task<string> SendRegister(string usrname, string password, string mail)
    {
        var response = await client.GetAsync(baseUrl + $"register/{usrname}/{password}/{mail}");
        return await response.Content.ReadAsStringAsync();
    }
    public async Task<string> SendPhase4(int bidValue)
    {
        var response = await client.GetAsync(baseUrl + $"{playerData1.playerId}/phase_4_input/{bidValue}");
        return await response.Content.ReadAsStringAsync();
    }


    // STUFF FOR COMMUNICATION WITH API ////////////////////////////////////////

    public async Task<string> ProcessRequestAsync(string request)
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

            authToken = await AuthenticateUser(username, password);
            Console.WriteLine($"Auth Token: {authToken}");
            playerID = GetPlayerID();
            return Login(username, password, authToken);
        }
        else if (request.StartsWith("/register"))
        {
            string username = request.Split('/')[2];
            string password = request.Split('/')[3];
            string email = request.Split('/')[4];

            return Register(username, password, email);
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
            return phase1Input(stormValue);
        }
        else if (request.StartsWith("/get_phase_2_info"))
        {
            return GetPhase2Info();
        }
        else if (request.StartsWith("/get_phase_3_info"))
        {
            return GetPhase3Info();
        }
        else if (request.StartsWith("/get_phase_4_info"))
        {
            return GetPhase4Info();
        }
        else if (request.StartsWith("/phase_4_input"))
        {
            int bidValue = int.Parse(request.Split('/')[2]);
            return phase4Input(bidValue);
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
    static string Login(string uusername, string password, string auth)
    {
        Console.WriteLine("Trying to log in {0} with pass {1}", uusername, password);
        // REPLACE THIS WITH WHATEVER IT IS WHEN WRONG LOGIN
        if (auth != "")
        {
            var playerData = new
            {
                username = uusername
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
    public string Register(string uusername, string password, string email)
    {
        Console.WriteLine("Trying to register in {0} with pass {1} and {2}", uusername, password, email);
        SendRegister(uusername, password, email);
        // REPLACE THIS
        if (true)
        {
            var playerData = new
            {
                message = "ok"
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
            turnId = playerData1.playerId,
            faction = playerData1.faction,
            spice = playerData1.spice,
            forcesReserve = playerData1.forcesReserve,
            forcesDeployed = 0,
            forcesDead = playerData1.forcesDead,
            // leaders = new[]
            // {
            //     new { name = "Leader1", faction = "Atreides", power = 1, @protected = false, status = "alive" },
            //     new { name = "Leader2", faction = "Atreides", power = 2, @protected = false, status = "alive" },
            //     new { name = "Leader3", faction = "Atreides", power = 3, @protected = false, status = "alive" },
            //     new { name = "Leader4", faction = "Atreides", power = 4, @protected = false, status = "alive" },
            //     new { name = "Leader5", faction = "Atreides", power = 5, @protected = false, status = "alive" }
            // },
            territories = new object[0],
            traitors = playerData1.traitorCards,
            treacheryCards = playerData1.treacheryCards

        };
        // Serialize playerData object to JSON
        return Newtonsoft.Json.JsonConvert.SerializeObject(playerDat);
    }
    public string GetOtherPlayersData()
    {
        // Retrieve player data from the database or another source
        // MAKE SURE TURN ID'S ARE IN ORDER ASCENDING !!!
        var otherPlayers = new[]
        {
             new { turnId = 2, username = "SomeDude", faction = "harkonnen", bot = "no", spice = "5" },
             new { turnId = 3, username = "Luffy", faction = "space_guild", bot = "no", spice = "5"  },
             new { turnId = 4, username = "Bahhhh", faction = "bene_gesserit", bot = "no", spice = "5"  },
             new { turnId = 5, username = "Noob420", faction = "emperor", bot = "no", spice = "5"  },
             new { turnId = 6, username = "Bot1", faction = "fremen", bot = "yes", spice = "5"  }
         };

        // Serialize playerData object to JSON
        return Newtonsoft.Json.JsonConvert.SerializeObject(otherPlayers);
    }
    public string GetPhaseInfo()
    {
        var phaseInfo = new
        {
            round = playerData1.round,
            phase = playerData1.phase
        };

        // Serialize playerData object to JSON
        return Newtonsoft.Json.JsonConvert.SerializeObject(phaseInfo);
    }
    public string GetMapSpice()
    {
        var mapSpice = new[]
        {
             new { sector = "meridian", spice = "5" },
             new { sector = "cielago-depression", spice = "3" }
         };

        // Serialize playerData object to JSON
        return Newtonsoft.Json.JsonConvert.SerializeObject(mapSpice);
    }
    public string GetPhase1Info()
    {
        var phase1Info = new
        {
            whichPlayers = new[]
            {
                 new { turnId = 2, hasPickedValue = "false" },
                 new { turnId = 1, hasPickedValue = "false" }
             },
            newStormPosition = playerData1.newStormPosition
        };

        if (playerData1.newStormPosition == -1 && playerData1.cursedTimer < 10)
        {
            playerData1.cursedTimer++;
        }
        else if (playerData1.newStormPosition == -1 && playerData1.phase1NeedInput == false)
        { playerData1.newStormPosition = 4; playerData1.cursedTimer = 0; playerData1.phase = 2; }
        // Serialize playerData object to JSON
        return Newtonsoft.Json.JsonConvert.SerializeObject(phase1Info);
    }
    public string phase1Input(int stormValue)
    {
        // send stormValue to server
        SendPhase1(stormValue);
        // response
        var validation = new
        {
            response = "ok"
        };
        playerData1.phase1NeedInput = false;
        playerData1.cursedTimer = 0;
        // Serialize playerData object to JSON
        return Newtonsoft.Json.JsonConvert.SerializeObject(validation);
    }
    public string GetPhase2Info()
    {
        var phase2Info = new
        {
            howMany = 2,
            whichTerritories = new[]
            {
                 new { name = "funeral-plain", addedSpice = 1 },
                 new { name = "the-great-flat", addedSpice = 2 }
             },
        };

        if (playerData1.cursedTimer < 20)
        {
            playerData1.cursedTimer++;
        }
        else
        {
            playerData1.phase = 3;
            Console.WriteLine("to phase 3 -----------------------");
            playerData1.cursedTimer = 0;
        }
        // Serialize playerData object to JSON
        return Newtonsoft.Json.JsonConvert.SerializeObject(phase2Info);
    }
    public string GetPhase3Info()
    {
        var phase2Info = new
        {
            howMany = 2,
            whichPlayers = new[]
            {
                 new { turnId = 1, addedSpice = 1 },
                 new { turnId = 3, addedSpice = 2 }
             },
        };

        if (playerData1.cursedTimer < 20)
        {
            playerData1.cursedTimer++;
        }
        else
        {
            playerData1.phase = 4;
            playerData1.cursedTimer = 0;
            Console.WriteLine("to phase 4 -----------------------");
        }
        // Serialize playerData object to JSON
        return Newtonsoft.Json.JsonConvert.SerializeObject(phase2Info);
    }
    public string GetPhase4Info()
    {
        var phase4Info = new
        {
            firstBidderTurnId = 3,
            lastBidder = playerData1.lastBidder,
            lastBid = playerData1.bidValue,
            biddedCardId = 2,
            bidStopped = false
        };

        if (playerData1.bidStopped == true && playerData1.cursedTimer < 10)
        { playerData1.cursedTimer++; }
        else if (playerData1.bidStopped == true && playerData1.cursedTimer > 9)
        {
            playerData1.cursedTimer = 0;
            playerData1.phase = 5;
        }

        // Serialize playerData object to JSON
        return Newtonsoft.Json.JsonConvert.SerializeObject(phase4Info);
    }
    public string phase4Input(int bidValue)
    {
        // send stormValue to server
        SendPhase4(bidValue);
        // response

        var validation = new
        {
            response = "ok"
        };
        playerData1.bidStopped = true;
        playerData1.bidValue = bidValue;
        playerData1.lastBidder = 1;

        // Serialize playerData object to JSON
        return Newtonsoft.Json.JsonConvert.SerializeObject(validation);
    }
}
