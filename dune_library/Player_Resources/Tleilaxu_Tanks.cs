using dune_library.Map_Resoures;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public class Tleilaxu_Tanks {
    public Tleilaxu_Tanks() {
      Forces = new();
      Non_Revivable_Generals = new Dictionary<Faction, ICollection<General>>(Enum.GetValues<Faction>().Select(faction =>
        new KeyValuePair<Faction, ICollection<General>>(faction, [])
      ));
      Revivable_Generals = new Dictionary<Faction, ICollection<General>>(Enum.GetValues<Faction>().Select(faction =>
        new KeyValuePair<Faction, ICollection<General>>(faction, [])
      ));
    }

    [JsonConstructor]
    public Tleilaxu_Tanks(
      Forces forces,
      IReadOnlyDictionary<Faction, ICollection<General>> non_revivable_generals,
      IReadOnlyDictionary<Faction, ICollection<General>> revivable_generals) {
      Forces = forces;
      Non_Revivable_Generals = non_revivable_generals;
      Revivable_Generals = revivable_generals;
    }

    public Forces Forces { get; }

    [JsonInclude]
    private IReadOnlyDictionary<Faction, ICollection<General>> Non_Revivable_Generals { get; }

    [JsonInclude]
    private IReadOnlyDictionary<Faction, ICollection<General>> Revivable_Generals { get; }

    public IEnumerable<General> Non_Revivable_Generals_Of(Faction faction) => Non_Revivable_Generals[faction];

    public IEnumerable<General> Revivable_Generals_Of(Faction faction) => Revivable_Generals[faction];

    public void Kill(int general_id) {
      General to_kill = general_id.To_General();
      Faction faction = to_kill.Faction;
      if (Non_Revivable_Generals[faction].Contains(to_kill) || Revivable_Generals[faction].Contains(to_kill)) {
        throw new ArgumentException("General " + to_kill + "is already dead");
      }
      Non_Revivable_Generals[faction].Add(to_kill);
      if (Non_Revivable_Generals[faction].Count == Generals_Manager.Nr_Of_Generals_Per_Faction) {
        Non_Revivable_Generals[faction].ForEach(general => Revivable_Generals[faction].Add(general));
        Non_Revivable_Generals[faction].Clear();
      }
    }

    public void Revive(int general_id) {
      General to_revive = general_id.To_General();
      Faction faction = to_revive.Faction;
      if (Revivable_Generals[faction].Contains(to_revive) == false) {
        throw new ArgumentException("General " + to_revive + "is not revivable");
      }
      Non_Revivable_Generals[faction].Remove(to_revive);
    }
  }
}
