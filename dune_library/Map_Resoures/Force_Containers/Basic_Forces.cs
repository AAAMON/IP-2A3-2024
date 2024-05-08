using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Map_Resoures.Force_Containers {
  public abstract class Basic_Forces<I> : Abstract_Simple_Forces<I> where I : Basic_Forces<I> {
    public Basic_Forces(uint forces_nr) : base(forces_nr) {}

    [JsonIgnore] public override uint Active_Forces_Count => Forces_Nr;
    [JsonIgnore] public override bool Is_Empty => Forces_Nr == 0;
  }
}
