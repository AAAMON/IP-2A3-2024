using dune_library.Utils;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public class Generals_Manager {
    public Generals_Manager() {
      Generals = new Dictionary<Faction, IReadOnlyCollection<General>> {
        [Faction.Atreides] = [
          new(Faction.Atreides, "Dr. Wellington Yueh", 1),
          new(Faction.Atreides, "Duncan Idaho", 2),
          new(Faction.Atreides, "Gurney Halleck", 4),
          new(Faction.Atreides, "Lady Jessica", 5),
          new(Faction.Atreides, "Thufir Hawat", 5),
        ],
        [Faction.Bene_Gesserit] = [
          new(Faction.Bene_Gesserit, "Wanna Yueh", 5),
          new(Faction.Bene_Gesserit, "Princess Irulan", 5),
          new(Faction.Bene_Gesserit, "Mother Ramallo", 5),
          new(Faction.Bene_Gesserit, "Margot Lady Fenring", 5),
          new(Faction.Bene_Gesserit, "Alia", 5),
        ],
        [Faction.Emperor] = [
          new(Faction.Emperor, "Bashar", 2),
          new(Faction.Emperor, "Burseg", 3),
          new(Faction.Emperor, "Caid", 3),
          new(Faction.Emperor, "Captain Aramsham", 5),
          new(Faction.Emperor, "Hasimir Fenring", 6),
        ],
        [Faction.Fremen] = [
          new(Faction.Fremen, "Jamis", 2),
          new(Faction.Fremen, "Shadout Mapes", 3),
          new(Faction.Fremen, "Otheym", 5),
          new(Faction.Fremen, "Chani", 6),
          new(Faction.Fremen, "Stilgar", 7),
        ],
        [Faction.Spacing_Guild] = [
          new(Faction.Spacing_Guild, "Guild Rep.", 1),
          new(Faction.Spacing_Guild, "Soo-Soo Sook", 2),
          new(Faction.Spacing_Guild, "Esmar Tuek", 3),
          new(Faction.Spacing_Guild, "Master Bewt", 3),
          new(Faction.Spacing_Guild, "Staban Tuek", 5),
        ],
        [Faction.Harkonnen] = [
          new(Faction.Harkonnen, "Umman Kudu", 1),
          new(Faction.Harkonnen, "Captain Iakin Nefud", 2),
          new(Faction.Harkonnen, "Piter de Vries", 3),
          new(Faction.Harkonnen, "Beast Rabban", 4),
          new(Faction.Harkonnen, "Feyd-Rautha", 6),
        ],
      };
      Generals_By_Id = Generals_By_Id_Default;
    }

    [JsonConstructor]
    public Generals_Manager(IReadOnlyDictionary<Faction, IReadOnlyCollection<General>> generals) {
      Generals = generals;
      Generals_By_Id = Generals_By_Id_Default;
    }

    [JsonIgnore]
    public const int Number_Of_Generals_Per_Faction = 5;

    public IReadOnlyDictionary<Faction, IReadOnlyCollection<General>> Generals { get; }

    [JsonIgnore]
    public IReadOnlyDictionary<int, General> Generals_By_Id { get; }
    [JsonIgnore]
    private IReadOnlyDictionary<int, General> Generals_By_Id_Default =>
      Generals.SelectMany(kvp => kvp.Value).Select(v => (v.Id, v)).ToDictionary();

    public IReadOnlyCollection<General> Of_Faction(Faction faction) => Generals[faction];

    public General With_Id(int id) => Generals_By_Id[id];

    public void Kill_General(General to_kill) {
      to_kill.Kill();
      if (Of_Faction(to_kill.Faction).Where(g => g.Is_Dead).Count() == Number_Of_Generals_Per_Faction) {
        Of_Faction(to_kill.Faction).ForEach(g => g.Make_Revivable());
      }
    }

    public void Kill_General(int id) => Kill_General(With_Id(id));
  }
}
