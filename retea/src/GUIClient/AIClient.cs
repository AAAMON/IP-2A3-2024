using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AIClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Should botname be an argument?
            await new ClientForAI("player1", "beneGesserit-med").Run(args);
        }
    }

    public class GameState
    {
        public string botname { get; set; }
        public string jsonString { get; set; }
    }
    public class ClientForAI
    {
        [JsonPropertyName("Factions_To_Move")]
        public List<bool> FactionsToMove { get; set; }

        private readonly string[] allFactionsInOrder = { "Atreides", "Bene_Gesserit", "Emperor", "Fremen", "Spacing_Guild", "Harkonnen" };

        private static readonly HttpClient client = new HttpClient();
        private static string baseUrl = "http://localhost:8000/";
        private static string serverUrl = "http://localhost:1234/";
        private static int moveID = 1;
        private string player = "player1";
        private int playerID = 1;
        private string botString;
        private string faction;
        public ClientForAI(string player, string botName)
        {
            this.player = player;
            this.playerID = GetPlayerID();
            this.botString = botName;
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
            client.DefaultRequestHeaders.Add("Authorization", player);
            client.DefaultRequestHeaders.Add("PlayerName", player);

            int indexer = 0;
            bool previousTryWorked = true;
            while (true)
            {
                if (indexer > 0)
                    return;
                string gameStateJson = await GetGamestate(player);
                using (StreamReader r = new StreamReader("perspectiva.json"))
                {
                    gameStateJson = r.ReadToEnd();
                }
                //Console.WriteLine(gameStateJson);
                try
                {
                    dynamic gameState = JObject.Parse(gameStateJson);
                    var factionsToMove = gameState.Factions_To_Move;
                    /// tot ok pana aici

                    int factionIndex = 0;
                    // Console.WriteLine(gameStateJson);
                    Console.WriteLine(factionsToMove);
                    foreach (var factionMoveValue in factionsToMove)
                    {
                        if (factionMoveValue == true && factionIndex < 7 && matchBotNameToFaction(allFactionsInOrder[factionIndex], botString))//stai oleaca cum stiu ordinea factiunilor sau cum apar in json?
                        {
                            indexer++;
                            Console.WriteLine("yay");
                            string moveJson = await PostMoveBody(gameStateJson, botString);
                            Console.WriteLine(moveJson);
                            string moveEndpoint = AIMoveFormatToGUIFormat(moveJson);
                            Console.WriteLine("the move is" + moveJson + " and move end point is" + moveEndpoint);

                            if (moveEndpoint.Contains("Error"))
                            {
                                continue;
                            }
                            previousTryWorked = true;
                            StringContent json = new StringContent("{\"isBot\": true}");
                            HttpResponseMessage response = await client.PostAsync(serverUrl + moveEndpoint, json);
                            response.EnsureSuccessStatusCode();

                        }
                        factionIndex++;
                    }
                }
                catch (Exception ex)
                {
                    if (previousTryWorked)
                    {
                        Console.WriteLine("[Error][There is no gamestate at /gamestate/" + player);
                        Console.WriteLine($"{ex.Message}");
                    }
                    previousTryWorked = false;
                }
            }
        }

        private void setBotName(string botName)
        {
            string[] bot_names = { "atreides-easy", "beneGesserit-easy", "emperor-easy", "fremen-easy", "harkonnen-easy", "spacingGuild-easy", "spacingGuild-med" };
            foreach (string str in bot_names)
                if (str.Contains(faction.ToLower()))
                {
                    botName = str;
                    return;
                }
            botName = "unknown";
            Console.WriteLine("[Error] Bot called without known faction");
        }
        private bool matchBotNameToFaction(string factionName, string botName)
        {
            string factionAux = factionName.ToLower().Trim().Replace("-", "").Replace("_", "");
            string botAux = botName.ToLower().Trim().Substring(0, botName.IndexOf("-"));
            // Console.WriteLine("Comparing (F)" + factionAux + "(B)" + botAux);

            return (botAux == factionAux);
        }
        private string AIMoveFormatToGUIFormat(string AIjson)
        {
            try
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
                        return "Error: Unknown action.";
                }
            }
            catch (Exception e)
            {
                return "Error:bad format, response is not a JSON object";
            }

        }

        private static async Task<string> GetGamestate(string authToken)
        {
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", authToken);
            var response = await client.GetAsync(serverUrl + $"gamestate/{authToken}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        private async Task<string> PostMoveBody(string gameStateJson, string bot_name)
        {
            try
            {
                GameState gameState = new GameState
                {
                    botname = bot_name,
                    jsonString = gameStateJson
                };

                var gJson = System.Text.Json.JsonSerializer.Serialize(gameState);
                var content = new StringContent(gJson, Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                var url = baseUrl + "get-move-body";
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(url),
                    Content = content
                };

                var response = await client.SendAsync(request);
                //response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "";
            }
        }

    }
}

/* [Request not used - Game logic needs to receive same format as GUI for movements]
private async Task PostMove(string moveJson)
{
    var content = new StringContent(moveJson, Encoding.UTF8, "application/json");
    var response = await client.PostAsync(baseUrl + $"{player}/move?moveID={moveID}", content);
    //response.EnsureSuccessStatusCode();
    moveID++;
}*/

/* [Request not used as it's unsafe]
    private async Task<string> GetMoveHeader(string gameStateJson, string bot_name)
    {
        var content = new StringContent(gameStateJson, Encoding.UTF8, "application/json");
        var url = baseUrl + "get-move-header?bot_name="+bot_name+"&game_state="+gameStateJson;
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url),
            Content = content
        };

        var response = await client.SendAsync(request);
        //response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }*/
