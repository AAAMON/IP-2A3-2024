using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dune_library.Utils {
  public abstract class Intel<C, K> where C : IEnumerable<K>, new() {
    public C Known { get; }

    public uint Unknown { get; private set; }

    public Intel() {
      Known = [];
      Unknown = 0;
    }

    [JsonConstructor]
    public Intel(C known, uint unknown) {
      Known = known;
      Unknown = unknown;
    }

    public abstract void Add_Knowledge(K knowledge_unit);

    public abstract void Lose_Knowledge(K knowledge_unit);

    public void Add_Unknown(uint unknown_additional_Knowledge_units) {
      Unknown += unknown_additional_Knowledge_units;
    }
  }
}
