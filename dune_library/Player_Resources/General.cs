using dune_library.Map_Resources;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public class General(string name, uint strength) {
    public string Name => name;

    public uint Strength => strength;

    public Option<Section> Location { get; set; } = None;

    #region Status Management

    public enum E_Status {
      Alive,
      Not_Revivable,
      Revivable,
    }

    public E_Status Status { get; private set; } = E_Status.Alive;

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
