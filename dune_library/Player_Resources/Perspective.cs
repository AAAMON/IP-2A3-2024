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

    public int Round { get; }

    public Option<Phase> Phase { get; }

    public Generals_Manager Generals_Manager { get; }

    public Alliances_Manager Alliances_Manager { get; }

    public Territory_Card Last_Spice_Card { get; }

    public Section_Forces Reserves { get; }

    public Tleilaxu_Tanks Tleilaxu_Tanks { get; }

    public Public_Faction_Knowledge_Manager Public_Faction_Knowledge_Manager { get; }

    public Special_Faction_Knowledge Special_Faction_Knowledge { get; }

    public Perspective(Faction faction, Game game) {
      Faction = faction;
      Battle_Wheels = game.Battle_Wheels;
      Map = game.Map;
      Round = game.Round;
      Phase = game.Phase;
      Generals_Manager = game.General_Manager;
      Alliances_Manager = game.Alliances_Manager;
      Last_Spice_Card = game.Last_Spice_Card;
      Reserves = game.Reserves;
      Tleilaxu_Tanks = game.Tleilaxu_Tanks;
      Public_Faction_Knowledge_Manager = game.Public_Faction_Knowledge_Manager;
      Special_Faction_Knowledge = game.Special_Faction_Knowledge_Manager.Of(faction);
    }

    [JsonConstructor]
    public Perspective(
      Faction faction,
      (Battle_Wheel A, Battle_Wheel B) battle_wheels,
      Map_Resources.Map map,
      int round,
      Option<Phase> phase,
      Generals_Manager generals_manager,
      Alliances_Manager alliances_manager,
      Territory_Card last_spice_card,
      Section_Forces reserves,
      Tleilaxu_Tanks tleilaxu_tanks,
      Public_Faction_Knowledge_Manager public_faction_knowledge_manager,
      Special_Faction_Knowledge special_faction_knowledge
    ) {
      Faction = faction;
      Battle_Wheels = battle_wheels;
      Map = map;
      Round = round;
      Phase = phase;
      Generals_Manager = generals_manager;
      Alliances_Manager = alliances_manager;
      Last_Spice_Card = last_spice_card;
      Reserves = reserves;
      Tleilaxu_Tanks = tleilaxu_tanks;
      Public_Faction_Knowledge_Manager = public_faction_knowledge_manager;
      Special_Faction_Knowledge = special_faction_knowledge;
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

    public static Perspective? DeserializeFromJson(string filePath) {
      try {
        var options = new JsonSerializerOptions { IncludeFields = true };
        options.Converters.Add(new Option_Json_Converter_Factory());
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<Perspective>(json, options);
      } catch (Exception e) {
        Console.WriteLine($"Error occurred during deserialization: {e}");
        return null;
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
