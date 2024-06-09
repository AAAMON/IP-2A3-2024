using dune_library.Map_Resoures;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static dune_library.Utils.Exceptions;

namespace dune_library.Player_Resources {
  public class Tleilaxu_Tanks {
    public Tleilaxu_Tanks(IReadOnlySet<Faction> factions_in_play) {
      Forces = new();
      Non_Revivable_Generals = factions_in_play.Select(faction =>
        new KeyValuePair<Faction, ICollection<General>>(faction, [])
      ).ToDictionary();
      Revivable_Generals = factions_in_play.Select(faction =>
        new KeyValuePair<Faction, ICollection<General>>(faction, [])
      ).ToDictionary();
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
      if (Non_Revivable_Generals.ContainsKey(faction) == false || Revivable_Generals.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
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
      if (Non_Revivable_Generals.ContainsKey(faction) == false || Revivable_Generals.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      if (Revivable_Generals[faction].Contains(to_revive) == false) {
        throw new ArgumentException("General " + to_revive + "is not revivable");
      }
      Non_Revivable_Generals[faction].Remove(to_revive);
    }
  }
}
