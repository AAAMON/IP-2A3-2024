using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  internal class Player_Info {
    public int Spice { get; set; }
    
    public int Reserves { get; }

    public int Dead_Forces { get; set; }

    public IList<General> Generals { get; set; }

    public IList<General> Traitors { get; set; }

    public IList<General> Discarded_Traitors { get; set; }

    public IReadOnlyCollection<Player_Presence> Player_Presences { get; set; } 
  }
}
