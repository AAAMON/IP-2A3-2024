using dune_library;
using dune_library.Phases;
using dune_library.Player_Resources;
using System.Text;

namespace MyApp {
  internal class Program {
    static void Main(string[] args) {
      Game game = new Game();
      Faction faction = Faction.Atreides;

      game.Generate_Perspective(faction);
      Perspective perspective = new Perspective(faction, game);
      perspective.SerializeToJson("perspective.json");
      var perspective2 = Perspective.DeserializeFromJson("perspective.json");
      PostJSONForPlayerI("perspective.json", 1);
      PostJSONForPlayerI("perspective.json", 2);
      PostJSONForPlayerI("perspective.json", 3);
      PostJSONForPlayerI("perspective.json", 4);
      PostJSONForPlayerI("perspective.json", 5);
      PostJSONForPlayerI("perspective.json", 6);
        }
    static async Task ForcedPost(string endpoint, string json)
    {
        HttpClient client = new HttpClient();
        string baseUrl = "http://localhost:1234/";
        client.PostAsync($"{baseUrl}{endpoint}", new StringContent(json, Encoding.UTF8, "application/json"));
    }
    static async Task PostJSONForPlayerI(string jsonContent, int i)
    {

        string filePath = Path.Combine("initialization", $"player{i}.json");
        File.WriteAllText(filePath, jsonContent);
        ForcedPost($"initialization/player{i}", jsonContent);
    }
    }
}