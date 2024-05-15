using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Decks.Spice {
  public class Territory_Card : Spice_Card {
    public uint Section_Position_In_List { get; }

    public Territory_Card(uint section_position_in_list) {
      Section_Position_In_List = section_position_in_list;
    }
  }
}
