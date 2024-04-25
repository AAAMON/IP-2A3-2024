﻿using dune_library.Player_Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map_Resources {
  internal class Polar_Sink : Territory {
    public override IReadOnlyList<Section> Sections { get; }
    public Polar_Sink(string name, IDictionary<Faction, ISet<Section>> presences) : base(name) {
      Sections = [new(18, this, presences)];
    }
  }
}
