using dune_library.Player_Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils {
  public interface I_Perspective_Generator {
    public Perspective Generate_Perspective(Player player);
  }
}
