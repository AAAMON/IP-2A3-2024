using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map {
  internal class Rock : Region {
    public override List<Section> Sections { get; }
    public Rock(string name, ushort first_sector, ushort sectors_spanned) : base(name) {
      Sections = Enumerable.Range(first_sector, first_sector + sectors_spanned)
                           .Select(sector => new Section(Map.To_Sector(sector), this))
                           .ToList();
    }
  }
}
