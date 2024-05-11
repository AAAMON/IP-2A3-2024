using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Treachery_Cards {
  public class Treachery_Card {
 
        public Treachery_Card(String name, String category, String type)
        {
            Name = name;
            Category = category;
            Type = type;
        }
        public String Name { get; }
        public String Category { get; }
        public String Type { get; }
    }
}
