using dune_library.Map_Resoures;
using dune_library.Player_Resources.Factions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public class Tleilaxu_Tanks {
    public Tleilaxu_Tanks() {
      Forces = new();
      /*Generals = new Dictionary<Faction_Class, IEnumerable<General>> {
        [Atreides.Instance] = [],
        [Bene_Gesserit.Instance] = [],
        [Emperor.Instance] = [],
        [Fremen.Instance] = [],
        [Spacing_Guild.Instance] = [],
        [Harkonnen.Instance] = [],
      };*/
      Generals = new Dictionary<Faction, IEnumerable<General>> {
        [Faction.Atreides] = [],
        [Faction.Bene_Gesserit] = [],
        [Faction.Emperor] = [],
        [Faction.Fremen] = [],
        [Faction.Spacing_Guild] = [],
        [Faction.Harkonnen] = [],
      };
    }

    [JsonConstructor]/*
    public Tleilaxu_Tanks(Section_Forces forces, IReadOnlyDictionary<Faction_Class, IEnumerable<General>> generals) {
      Forces = forces;
      Generals = generals;
    }*/
    public Tleilaxu_Tanks(Section_Forces forces, IReadOnlyDictionary<Faction, IEnumerable<General>> generals) {
      Forces = forces;
      Generals = generals;
    }

    public Section_Forces Forces { get; }

    [JsonInclude]
    /*private IReadOnlyDictionary<Faction_Class, IEnumerable<General>> Generals { get; }*/
    private IReadOnlyDictionary<Faction, IEnumerable<General>> Generals { get; }

    /*public IEnumerable<General> Generals_Of(Faction_Class faction) => Generals[faction];*/
    public IEnumerable<General> Generals_Of(Faction faction) => Generals[faction];
  }
}
