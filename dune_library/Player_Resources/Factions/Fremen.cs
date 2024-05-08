using dune_library.Map_Resoures.Force_Containers;
using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources.Factions {
  public class Fremen : Faction_Class {
    private Fremen() { }
    private static Option<Fremen> _instance;
    public static Fremen Instance {
      get {
        if (_instance == None) {
          _instance = Option<Fremen>.Some(new());
        }
        return _instance.ValueUnsafe();
      }
    }

    public override IEnumerable<General> Default_Generals => [
      Generals.Jamis,
      Generals.Shadout_Mapes,
      Generals.Otheym,
      Generals.Chani,
      Generals.Stilgar,
    ];

    public new class Forces : Abstract_Forces_With_Special<Forces> {
      public Forces(uint normal, uint fedaykin) : base(normal, fedaykin) { }

      public uint Fedaykin => Special;

      public override void Remove_By_Storm(Forces graveyard) =>
        Transfer_To(graveyard, Normal - Normal / 2, Special - Special / 2);

      public static implicit operator Forces((uint normal, uint fedaykin) value) =>
        new(value.normal, value.fedaykin);
    }
  }
}
