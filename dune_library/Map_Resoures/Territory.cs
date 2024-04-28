using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resources {
  internal abstract class Territory(string name) {
    private static int counter = 0;

    public int Id { get; } = counter++;

    public string Name { get; } = name;

    public abstract IReadOnlyList<Section> Sections { get; }
  }
}
