using dune_library.Map_Resources;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using LanguageExt.UnsafeValueAccess;
using System.Reflection.Metadata.Ecma335;

namespace dune_library.Player_Resources {
  public class General {
    [JsonIgnore]
    private static int counter = 0;

    public int Id { get; }

    public General(Faction faction, string name, int strength) {
      Id = counter++;
      Faction = faction;
      Name = name;
      Strength = strength;
    }

    public Faction Faction { get; }

    public string Name { get; }

    public int Strength { get; }
  }
}
