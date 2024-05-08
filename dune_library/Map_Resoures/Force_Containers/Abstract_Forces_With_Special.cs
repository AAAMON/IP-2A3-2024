using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Map_Resoures.Force_Containers {
  public abstract class Abstract_Forces_With_Special<I> : Abstract_Forces<I> where I : Abstract_Forces_With_Special<I> {
    public Abstract_Forces_With_Special(uint normal, uint special) {
      Normal = normal;
      Special = special;
    }

    public uint Normal { get; private set; }
    protected uint Special { get; private set; }
    [JsonIgnore] public override uint Active_Forces_Count => Normal + Special;
    [JsonIgnore] public override bool Is_Empty => Active_Forces_Count == 0;

    public void Transfer_To(I destination, uint to_transfer_normal, uint to_transfer_special) {
      Remove(to_transfer_normal, to_transfer_special);
      destination.Add(to_transfer_normal, to_transfer_special);
    }

    public override void Transfer_To(I destination, params uint[] to_transfer) {
      if (to_transfer.Length != 2) {
        throw new ArgumentException("expected two uint arguments");
      }
      Transfer_To(destination, to_transfer[0], to_transfer[1]);
    }

    public abstract override void Remove_By_Storm(I graveyard);

    private void Add(uint normal_to_add, uint special_to_add) {
      Normal += normal_to_add;
      Special += special_to_add;
    }
    private void Remove(uint normal_to_remove, uint special_to_remove) {
      if (Normal < normal_to_remove) {
        throw new ArgumentException("Tried to remove " + normal_to_remove + " normal forces, while there are " + Normal + " normal forces present", nameof(normal_to_remove));
      }
      if (Special < special_to_remove) {
        throw new ArgumentException("Tried to remove " + special_to_remove + " special forces, while there are " + Special + " special forces present", nameof(special_to_remove));
      }
      Normal -= normal_to_remove;
      Special -= special_to_remove;
    }

    public void Deconstruct(out uint normal, out uint special) {
      normal = Normal;
      special = Special;
    }
    //implement implicit cast from uint pair to inheriting class
  }
}
