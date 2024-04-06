using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Map {
  internal class Region {
    public Region(string name) {
      Name = name;
      Neighboring_Regions = [];
      can_add_neighbors = true;
      sections = [];
    }

    public string Name { get; }

    protected List<Section> sections;
    // sections will always be unique, since it's modified in inheriting classes,
    // so there's no point in storing them in a HashSet

    public List<Section> Sections => sections;
    
    public HashSet<Region> Neighboring_Regions { get; }

    private bool can_add_neighbors;
    
    public void Block_Adding_Neighbors() {
      can_add_neighbors = false;
    }

    public bool Add_Neighbor(Region other) {
      if (can_add_neighbors == false) { return false; }
      return Neighboring_Regions.Add(other);
    }
  }
}
