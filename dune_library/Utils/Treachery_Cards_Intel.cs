using dune_library.Treachery_Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils {
  internal class Treachery_Cards_Intel : Intel<Occurence_Dict<Treachery_Card>, Treachery_Card> {
    public Treachery_Cards_Intel() : base() {
    }

    public Treachery_Cards_Intel(Occurence_Dict<Treachery_Card> known, uint unknown) : base(known, unknown) {
    }

    public override void Add_Knowledge(Treachery_Card knowledge_unit) {
      Known.Add(knowledge_unit);
    }

    public override void Lose_Knowledge(Treachery_Card knowledge_unit) {
      Known.Remove(knowledge_unit);
    }
  }
}
