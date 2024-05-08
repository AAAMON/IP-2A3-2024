using dune_library.Map_Resoures.Force_Containers;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.UnsafeValueAccess;

namespace dune_library.Player_Resources.Factions {
  public class Harkonnen : Faction_Class {
    private Harkonnen() { }
    private static Option<Harkonnen> _instance;
    public static Harkonnen Instance {
      get {
        if (_instance == None) {
          _instance = Option<Harkonnen>.Some(new());
        }
        return _instance.ValueUnsafe();
      }
    }

    public override IEnumerable<General> Default_Generals => [
      Generals.Umman_Kudu,
      Generals.Captain_Iakin_Nefud,
      Generals.Piter_de_Vries,
      Generals.Beast_Rabban,
      Generals.Feyd_Rautha,
    ];

    public new class Forces : Basic_Forces<Forces> {
      public Forces(uint forces_nr) : base(forces_nr) { }

      public static implicit operator Forces(uint forces_nr) => new(forces_nr);
    }
  }
}
