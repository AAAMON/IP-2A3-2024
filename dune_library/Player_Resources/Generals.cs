using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  internal class Generals(Faction faction) {
    private readonly ICollection<General> generals = faction switch {
      Faction.Atreides => [
        new("Dr. Wellington Yueh", 1),
        new("Duncan Idaho", 2),
        new("Gurney Halleck", 4),
        new("Lady Jessica", 5),
        new("Thufir Hawat", 5),
      ],
      Faction.Bene_Gesserit => [
        new("Wanna Yueh", 5),
        new("Princess Irulan", 5),
        new("Mother Ramallo", 5),
        new("Margot Lady Fenring", 5),
        new("Alia", 5),
      ],
      Faction.Emperor => [
        new("Bashar", 2),
        new("Burseg", 3),
        new("Caid", 3),
        new("Captain Aramsham", 5),
        new("Hasimir Fenring", 6),
      ],
      Faction.Fremen => [
        new("Jamis", 2),
        new("Shadout Mapes", 3),
        new("Otheym", 5),
        new("Chani", 6),
        new("Stilgar", 7),
      ],
      Faction.Spacing_Guild => [
        new("Guild Rep.", 1),
        new("Soo-Soo Sook", 2),
        new("Esmar Tuek", 3),
        new("Master Bewt", 3),
        new("Staban Tuek", 5),
      ],
      Faction.Harkonnen => [
        new("Umman Kudu", 1),
        new("Captain Iakin Nefud", 2),
        new("Piter de Vries", 3),
        new("Beast Rabban", 4),
        new("Feyd-Rautha", 6),
      ],
      _ => throw new ArgumentException("invalid faction"),
    };

    

  }
}
