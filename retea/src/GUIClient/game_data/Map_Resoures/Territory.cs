using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resources {
  public abstract class Territory(string name, uint id) {
    public uint Id { get; } = id;

    public string Name { get; } = name;

    public abstract IReadOnlyList<Section> Sections { get; }
  }
}
