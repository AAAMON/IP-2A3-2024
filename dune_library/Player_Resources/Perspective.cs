using dune_library.Map_Resources;
using dune_library.Map_Resoures;
using dune_library.Phases;
using dune_library.Spice;
using dune_library.Utils;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public class Perspective {
    public Faction Faction { get; }

    public (Battle_Wheel A, Battle_Wheel B) Battle_Wheels { get; }

    public Map_Resources.Map Map { get; }

    public uint Round { get; }

    public Option<Phase> Phase { get; }

    public Player_Markers Player_Markers { get; }

    public Alliances Alliances { get; }

    public Territory_Card Last_Spice_Card { get; }

    public Tleilaxu_Tanks Tleilaxu_Tanks { get; }

    public Special_Faction_Knowledge Special_Faction_Knowledge { get; } // can be destructured into individual properties

    public Perspective(Faction faction, Game game) {
      Faction = faction;
      Battle_Wheels = game.Battle_Wheels;
      Map = game.Map;
      Round = game.Round;
      Phase = game.Phase;
      Player_Markers = game.Player_Markers;
      Alliances = game.Alliances;
      Last_Spice_Card = game.Last_Spice_Card;
      Tleilaxu_Tanks = game.Tleilaxu_Tanks;
      // Special_Faction_Knowledge = game.Special_Faction_Knowledge_Manager.Of(faction);
    }

    public void SerializeToJson(string filePath) {
      try {
          var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
          string json = JsonSerializer.Serialize(this, options);
          File.WriteAllText(filePath, json);
          Console.WriteLine("Serialization successful.");
      } catch (Exception e) {
          Console.WriteLine($"Error occurred during serialization: {e}");
      }
    }

    /*public static Perspective? DeserializeFromJson(string filePath) {
      try {
        var options = new JsonSerializerOptions { IncludeFields = true };
        options.Converters.Add(new Option_Json_Converter_Factory());
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<Perspective>(json, options);
      } catch (Exception e) {
        Console.WriteLine($"Error occurred during deserialization: {e}");
        return null;
      }
    }*/ /* !!! DESERIALIZATION NO LONGER NEEDED !!! */

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
