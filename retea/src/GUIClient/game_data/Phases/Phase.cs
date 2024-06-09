using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Phases {
  public abstract class Phase {
    public abstract void Play_Out();
    public abstract string name { get; }
    public abstract string moment { get; protected set; }
  }
}
