using dune_library.Map_Resoures.Force_Containers;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.SomeHelp;
using LanguageExt.UnsafeValueAccess;

namespace dune_library.Player_Resources.Factions {
  public class Atreides : Faction_Class {
    private Atreides() { }
    
    private static Option<Atreides> _instance;
    public static Atreides Instance {
      get {
        if (_instance == None) {
          _instance = Option<Atreides>.Some(new());
        }
        return _instance.ValueUnsafe();
      }
    }

    public override IEnumerable<General> Default_Generals => [
      Generals.Dr_Wellington_Yueh,
      Generals.Duncan_Idaho,
      Generals.Gurney_Halleck,
      Generals.Lady_Jessica,
      Generals.Thufir_Hawat,
    ];

    public new class Forces : Basic_Forces<Forces> {
      public Forces(uint forces_nr) : base(forces_nr) { }

      public static implicit operator Forces(uint forces_nr) => new(forces_nr);
    }
  }
}
