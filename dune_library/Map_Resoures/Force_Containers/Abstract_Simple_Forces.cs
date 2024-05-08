using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Map_Resoures.Force_Containers {
  public abstract class Abstract_Simple_Forces<I> : Abstract_Forces<I> where I : Abstract_Simple_Forces<I> {
    public Abstract_Simple_Forces(uint forces_nr) {
      Forces_Nr = forces_nr;
    }

    public uint Forces_Nr { get; private set; }
    [JsonIgnore] public abstract override uint Active_Forces_Count { get; }
    [JsonIgnore] public abstract override bool Is_Empty { get; }

    public void Transfer_To(I destination, uint to_transfer) {
      Remove(to_transfer);
      destination.Add(to_transfer);
    }

    public override void Transfer_To(I destination, params uint[] to_transfer) {
      if (to_transfer.Length != 1) {
        throw new ArgumentException("expected one uint arguments");
      }
      Transfer_To(destination, to_transfer[0]);
    }

    public override void Remove_By_Storm(I graveyard) => Transfer_To(graveyard, Forces_Nr);

    private void Add(uint to_add) => Forces_Nr += to_add;
    private void Remove(uint to_remove) {
      if (Forces_Nr < to_remove) {
        throw new ArgumentException("Tried to remove " + to_remove + " normal forces, while there are " + Forces_Nr + " normal forces present", nameof(to_remove));
      }
      Forces_Nr -= to_remove;
    }
    public static implicit operator uint(Abstract_Simple_Forces<I> obj) => obj.Forces_Nr;
    //implement implicit cast from uint to inheriting class and vice-versa
  }
}
