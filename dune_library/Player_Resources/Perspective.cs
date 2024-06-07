using dune_library.Map_Resources;
using dune_library.Map_Resoures;
using dune_library.Phases;
using dune_library.Utils;
using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using dune_library.Decks.Spice;
using dune_library.Player_Resources;


namespace dune_library.Player_Resources
{
    public class Perspective
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlySet<Faction>? Free_Factions { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlySet<Faction>? Taken_Factions { get; }

        public Option<Faction> Faction { get; }

        public Game_Winners Game_Winners { get; }

        public bool[] Factions_To_Move { get; } = new bool[6];

        public (Battle_Wheel A, Battle_Wheel B) Battle_Wheels { get; }

        public Map_Resources.Map Map { get; }

        public uint Round { get; }

        public Option<Phase> Phase { get; }

        public Option<Either<Player_Markers_Manager, Final_Player_Markers>> Player_Markers { get; }

        public Option<Alliances> Alliances { get; }

        public Option<Spice_Card> Last_Spice_Card { get; }

        public Option<Forces> Reserves { get; }

        public Highest_Bid Highest_Bid { get; }

        public Faction_Battles Faction_Battles { get; }
        public Option<Tleilaxu_Tanks> Tleilaxu_Tanks { get; }
        public Option<I_Faction_Knowledge_Read_Only> Faction_Knowledge { get; } // can be destructured into individual properties

        public Perspective(
          Player player,
          (Battle_Wheel A, Battle_Wheel B) battle_wheels,
          Map_Resources.Map map,
          uint round,
          Option<Phase> phase,
          Either<Factions_Distribution_Manager, Final_Factions_Distribution> factions_distribution,
          Option<Either<Player_Markers_Manager, Final_Player_Markers>> player_markers,
          Option<Alliances> alliances,
          Option<Forces> reserves,
          Option<Tleilaxu_Tanks> tleilaxu_tanks,
          Option<Knowledge_Manager> knowledge_manager,
          Option<Spice_Card> last_spice_card,
          Highest_Bid HighestBid,
          bool[] Factions_To_Move,
          Game_Winners Game_Winners,
          Faction_Battles Faction_Battles
           )
        {
            if (factions_distribution.IsLeft)
            {
                Free_Factions = factions_distribution.Left().Free_Factions;
                Taken_Factions = factions_distribution.Left().Taken_Factions;
                Faction = factions_distribution.Left().Faction_Of(player);
            }
            else
            {
                Free_Factions = null;
                Taken_Factions = null;
                Faction = factions_distribution.Right().Faction_Of(player);
            }
            Battle_Wheels = battle_wheels;
            this.Game_Winners = Game_Winners;
            Map = map;
            Round = round;
            Phase = phase;
            Player_Markers = player_markers;
            Alliances = alliances;
            Reserves = reserves;
            Last_Spice_Card = last_spice_card;
            Tleilaxu_Tanks = tleilaxu_tanks;
            this.Highest_Bid = HighestBid;
            this.Factions_To_Move = Factions_To_Move;
            this.Faction_Battles = Faction_Battles;

            Faction_Knowledge = knowledge_manager.Map(km => km.Of(Faction.Value())); //if 'knowledge_manager' is some, then factions are initialized, and 'Faction' is some
        }

        public void SerializeToJson(string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
                options.Converters.Add(new JsonStringEnumConverter());
                options.Converters.Add(new SpiceCardConverter());

                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(filePath, json);
                Console.WriteLine("Serialization successful.");
            }
            catch (Exception e)
            {
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
