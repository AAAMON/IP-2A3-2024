using dune_library.Map_Resources;
using dune_library.Player_Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace dune_library.Map_Resoures {
  public class With_Spice(uint sector, Territory territory, uint id, uint spice_capacity) : Section(sector, territory, id) {
    public uint Spice_Capacity { get; } = spice_capacity;

    public uint Spice_Avaliable { get; private set; } = 0;

    public uint Take_Spice(uint how_much_can_be_carried_by_forces) {
      uint to_take = uint.Min(Spice_Avaliable, how_much_can_be_carried_by_forces);
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
