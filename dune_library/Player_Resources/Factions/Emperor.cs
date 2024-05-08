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
  public class Emperor : Faction_Class {
    private Emperor() { }
    private static Option<Emperor> _instance;
    public static Emperor Instance {
      get {
        if (_instance == None) {
          _instance = Option<Emperor>.Some(new());
        }
        return _instance.ValueUnsafe();
      }
    }

    public override IEnumerable<General> Default_Generals => [
      Generals.Bashar,
      Generals.Burseg,
      Generals.Caid,
      Generals.Captain_Aramsham,
      Generals.Hasimir_Fenring,
    ];

    public new class Forces : Abstract_Forces_With_Special<Forces> {
      public Forces(uint normal, uint sardaukar) : base(normal, sardaukar) { }

      public uint Sardaukar => Special;

      public override void Remove_By_Storm(Forces graveyard) => Transfer_To(graveyard, Normal, Special);

      public static implicit operator Forces((uint normal, uint sardaukar) value) =>
        new(value.normal, value.sardaukar);
    }
  }
}
