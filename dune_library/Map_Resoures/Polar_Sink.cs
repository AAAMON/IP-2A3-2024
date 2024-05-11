using dune_library.Player_Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resources {
  public class Polar_Sink : Territory {
    public override IReadOnlyList<Section> Sections { get; }
    public Polar_Sink(string name, uint id, ref uint section_counter) : base(name, id) {
      Sections = [new(18, this, section_counter++)];
    }
  }
}
