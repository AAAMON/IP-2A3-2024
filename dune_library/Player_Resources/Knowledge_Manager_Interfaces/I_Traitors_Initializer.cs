using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources.Knowledge_Manager_Interfaces {
  public interface I_Traitors_Initializer {
    public void Init_Traitors(Faction faction, IReadOnlyList<General> traitors, IReadOnlyList<General> discarded_traitors);
  }
}
