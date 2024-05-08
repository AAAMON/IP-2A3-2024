using dune_library.Map_Resoures.Force_Containers;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LanguageExt.UnsafeValueAccess;

namespace dune_library.Player_Resources.Factions {
  public class Bene_Gesserit : Faction_Class {
    private Bene_Gesserit() { }
    private static Option<Bene_Gesserit> _instance;
    public static Bene_Gesserit Instance {
      get {
        if (_instance == None) {
          _instance = Option<Bene_Gesserit>.Some(new());
        }
        return _instance.ValueUnsafe();
      }
    }

    public override IEnumerable<General> Default_Generals => [
      Generals.Wanna_Yueh,
      Generals.Princess_Irulan,
      Generals.Mother_Ramallo,
      Generals.Margot_Lady_Fenring,
      Generals.Alia,
    ];

    public new class Forces : Abstract_Simple_Forces<Forces> {
      public Forces(uint forces_nr, bool are_advisors = false) : base(forces_nr) {
        Are_Advisors = are_advisors;
      }

      public bool Are_Advisors { get; private set; }
      [JsonIgnore] public override uint Active_Forces_Count => Are_Advisors ? 0 : Forces_Nr;
      [JsonIgnore] public override bool Is_Empty => Forces_Nr == 0;

      public void Deconstruct(out uint forces_nr, out bool are_advisors) {
        forces_nr = Forces_Nr;
        are_advisors = Are_Advisors;
      }
      public static implicit operator Forces((uint forces_nr, bool are_advisors) value) => new(value.forces_nr, value.are_advisors);
    }
  }
}
