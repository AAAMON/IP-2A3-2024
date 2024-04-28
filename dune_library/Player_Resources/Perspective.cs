using dune_library.Map_Resources;
using dune_library.Phases;
using dune_library.Spice;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public class Perspective(Game game, Faction faction) {
    public Faction Faction { get; } = faction;

    public Map_Resources.Map Map => game.Map;

    public int Storm_Sector => Map.Storm_Sector;

    public int Round => game.Round;

    public Option<Phase> Phase => game.Phase;

    public IDictionary<Faction, int> Assigned_Sector => game.Assigned_Sector;

    public IDictionary<Faction, Player_Info> Player_Infos { get; } =
      game.Player_Infos.Select(e =>
        new KeyValuePair<Faction, Player_Info>(e.Key, e.Key == faction ? e.Value : new Other_Player_Info(e.Value))
      ).ToDictionary();

    //public Own_Player_Info Own_Player_Info => (Own_Player_Info)Player_Infos[Faction];

    // public Territory_Card Last_Spice_Card { get; set; }

    public void SerializeToJson(string filePath)
     {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(filePath, json);
            Console.WriteLine("Serialization successful.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred during serialization: {ex.Message}");
        }
     }

     /*public static Perspective DeserializeFromJson(string filePath)
     {
        try
        {
            string jsonData = File.ReadAllText(filePath);
            Perspective deserializedObject = JsonSerializer.Deserialize<Perspective>(jsonData);
            return deserializedObject;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred during deserialization: {ex.Message}");
            return default;
        }
     }*/

  }
}
