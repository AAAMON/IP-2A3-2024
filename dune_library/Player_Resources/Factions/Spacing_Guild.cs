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
  public class Spacing_Guild : Faction_Class {
    private Spacing_Guild() { }
    private static Option<Spacing_Guild> _instance;
    public static Spacing_Guild Instance {
      get {
        if (_instance == None) {
          _instance = Option<Spacing_Guild>.Some(new());
        }
        return _instance.ValueUnsafe();
      }
    }

    public override IEnumerable<General> Default_Generals => [
      Generals.Guild_Rep,
      Generals.Soo_Soo_Sook,
      Generals.Esmar_Tuek,
      Generals.Master_Bewt,
      Generals.Staban_Tuek,
    ];

    public new class Forces : Basic_Forces<Forces> {
      public Forces(uint forces_nr) : base(forces_nr) { }

      public static implicit operator Forces(uint forces_nr) => new(forces_nr);
    }
  }
}
