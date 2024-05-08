using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Map_Resoures.Force_Containers {
  public abstract class Abstract_Forces<I> : I_Countable, I_Emptiness_Checkable, I_Removable_By_Storm<I> where I : Abstract_Forces<I> {
    [JsonIgnore] public abstract uint Active_Forces_Count { get; }
    [JsonIgnore] public abstract bool Is_Empty { get; }

    public abstract void Transfer_To(I destination, params uint[] to_transfer);

    public abstract void Remove_By_Storm(I graveyard);
  }
}
