using dune_library.Player_Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Utils {
  public class Traitors_Intel : Intel<List<General>, General> {
    public Traitors_Intel() : base() {
    }

    [JsonConstructor]
    public Traitors_Intel(List<General> known, uint unknown) : base(known, unknown) {
    }

    public override void Add_Knowledge(General knowledge_unit) {
      Known.Add(knowledge_unit);
    }

    public override void Lose_Knowledge(General knowledge_unit) {
      Known.Remove(knowledge_unit);
    }
  }
}
