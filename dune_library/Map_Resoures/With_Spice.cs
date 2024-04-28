using dune_library.Map_Resources;
using dune_library.Player_Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resoures {
  internal class With_Spice(int sector, Territory territory, int spice_capacity) : Section(sector, territory) {
    public int Spice_Capacity => spice_capacity;

    public int Spice_Avaliable { get; private set; } = 0;

    public int Take_Spice(int amount) {
      int to_take = int.Min(Spice_Avaliable, amount);
      Spice_Avaliable -= to_take;
      return to_take;
    }

    public void Add_Spice() {
      Spice_Avaliable = Spice_Capacity;
    }

    public override void Delete_Spice() {
      Spice_Avaliable = 0;
    }
  }
}
