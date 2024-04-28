using dune_library.Map_Resources;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  internal class General(Faction faction, string name, int strength) {
    private static int counter = 0;

    public int Id { get; } = counter++;

    public readonly Faction Faction = faction;

    public readonly string Name = name;

    public readonly int Strength = strength;

    public Option<Section> Location { get; set; } = None;

    #region Status Management

    public enum E_Status {
      Alive,
      Not_Revivable,
      Revivable,
    }

    private E_Status Status { get; set; } = E_Status.Alive;

    public bool Is_Alive => Status == E_Status.Alive;

    public bool Is_Dead => !Is_Alive;

    public bool Is_Revivable => Status == E_Status.Revivable;

    public bool Is_Not_Revivable => Status == E_Status.Not_Revivable;

    public void Kill() {
      if (Status == E_Status.Alive) {
        Status = E_Status.Not_Revivable;
      }
    }

    public void Make_Revivable() {
      if (Status == E_Status.Not_Revivable) {
        Status = E_Status.Revivable;
      }
    }

    public void Revive() {
      if (Status == E_Status.Revivable) {
        Status = E_Status.Alive;
      }
    }

    #endregion
  }
}
