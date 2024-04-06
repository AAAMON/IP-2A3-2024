using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map {
  internal class Polar_Sink : Region {
    public Polar_Sink(string name) : base(name) {
      sections = [new(18)];
    }
  }
}
