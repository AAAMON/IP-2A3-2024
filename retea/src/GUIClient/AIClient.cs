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
        private string player;
        private int playerID ;
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
                /*(  using (StreamReader r = new StreamReader("perspectiva.json")) [only for testing purpose]
                {
                    gameStateJson = r.ReadToEnd();
                }
                /*/
                try
                {
                    dynamic gameState = JObject.Parse(gameStateJson);
                    var factionsToMove = gameState.Factions_To_Move;

                    int factionIndex = 0;
                    Console.WriteLine(factionsToMove);
                    foreach (var factionMoveValue in factionsToMove)
                    {
                        if (factionMoveValue == true && factionIndex < 7 && matchBotNameToFaction(allFactionsInOrder[factionIndex], botString))
                        {
                            indexer++;
                            
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

            return (botAux == factionAux);
        }
        private string AIMoveFormatToGUIFormat(string AIjson)
        {

            JObject json = JObject.Parse(AIjson);
            string action = json["action"].ToString();
            string playerId = string.parse(playerId);

            switch (action)
            {
                case "predict":
                    string faction = json["faction"].ToString();
                    int round = int.Parse(json["round"].ToString());
                    return $"/{playerId}/setup/number/{faction}";

                case "pick_traitor":
                    string traitorName = json["name"].ToString();
                    return $"/{playerId}/setup/traitor_name/{traitorName}";

                case "spawn":
                    int sectionId = int.Parse(json["section_id"].ToString());
                    int troopNumber = int.Parse(json["troop_number"].ToString());
                    return $"/{playerId}/setup/{sectionId}/{troopNumber}";

                case "Storm":
                    int stormValue = int.Parse(json["value"].ToString());
                    return $"/{playerId}/phase_1_input/{stormValue}";

                case "bid":
                    int bidValue = int.Parse(json["value"].ToString());
                    return $"/{playerId}/phase_4_input/{bidValue}";

                case "pass":
                    string phase = json["phase"].ToString();
                    return $"/{playerId}/phase_{phase}_input/pass";

                case "revive":
                    int reviveTroopNumber = int.Parse(json["value"].ToString());
                    string generalName = json["general_name"].ToString();
                    return $"/{playerId}/phase_5_input/{reviveTroopNumber}/{generalName}";

                case "shipment":
                    int shipmentSectionId = int.Parse(json["section"].ToString());
                    int shipmentTroopNumber = int.Parse(json["value"].ToString());
                    return $"/{playerId}/phase_6_input/1/{shipmentSectionId}/{shipmentTroopNumber}";

                case "movement":
                    int fromSectionId = int.Parse(json["source_section"].ToString());
                    int toSectionId = int.Parse(json["destination_terittory"].ToString());
                    int movementTroopNumber = int.Parse(json["value"].ToString());
                    return $"/{playerId}/phase_6_input/2/{fromSectionId}/{toSectionId}/{movementTroopNumber}";

                case "bene_shipment":
                    string answer = json["answer"].ToString();
                    return $"/bene_player_id/phase_6_input/{answer}";

                case "choose_battle":
                    int territoryId = int.Parse(json["teritory_id"].ToString());
                    string opponentFaction = json["opponent_faction"].ToString();
                    return $"/{playerId}/phase_7_input/{territoryId}/{opponentFaction}";

                case "battle":
                    int forces = int.Parse(json["troops"].ToString());
                    string usedGeneral = json["used_general"].ToString();
                    string attackCard = json["attack_card"].ToString();
                    string defenseCard = json["defense_card"].ToString();
                    return $"/{playerId}/phase_7_input/{forces}/{usedGeneral}/{attackCard}/{defenseCard}";

                default:
                    return $"/{playerId}/unknown"; // Handle unknown actions appropriately
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
                response.EnsureSuccessStatusCode();
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
