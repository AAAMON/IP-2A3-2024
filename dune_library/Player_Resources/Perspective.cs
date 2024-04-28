using dune_library.Map_Resources;
using dune_library.Phases;
using dune_library.Spice;
using dune_library.Utils;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  internal class Perspective {
    public Faction Faction { get; }

    public (Battle_Wheel A, Battle_Wheel B) Battle_Wheels { get; }

    public Map_Resources.Map Map { get; }

    public int Round { get; }

    public Option<Phase> Phase { get; }

    public Generals_Manager General_Manager { get; }

    public Alliances_Manager Alliances_Manager { get; }

    public Territory_Card Last_Spice_Card { get; }

    public IDictionary<Faction, Public_Faction_Knowledge> Public_Faction_Knowledge { get; }

    public Special_Faction_Knowledge Special_Faction_Knowledge { get; }

    public Perspective(Faction faction, Game game) {
      Faction = faction;
      Battle_Wheels = game.Battle_Wheels;
      Map = game.Map;
      Phase = game.Phase;
      General_Manager = game.General_Manager;
      Alliances_Manager = game.Alliances_Manager;
      Last_Spice_Card = game.Last_Spice_Card;
      Public_Faction_Knowledge = game.Public_Faction_Knowledge;
      Special_Faction_Knowledge = game.Special_Faction_Knowledge[faction];
    }
  }
}
