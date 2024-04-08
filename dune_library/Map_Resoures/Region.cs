using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resources {
  internal abstract class Region(string name) {
    public string Name { get; } = name;

    public abstract List<Section> Sections { get; }
  }
}
