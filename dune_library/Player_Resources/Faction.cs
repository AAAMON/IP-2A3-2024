using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dune_library.Map_Resoures.Force_Containers;
using dune_library.Player_Resources.Factions;
using dune_library.Treachery_Cards;

namespace dune_library.Player_Resources {
  public enum Faction {
    Atreides,
    Bene_Gesserit,
    Emperor,
    Fremen,
    Spacing_Guild,
    Harkonnen,
  }
  public abstract class Faction_Class {
    public abstract class Forces : Abstract_Forces<Forces> {}

    public abstract IEnumerable<General> Default_Generals { get; }

  }
}
