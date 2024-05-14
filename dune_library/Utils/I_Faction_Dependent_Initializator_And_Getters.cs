using dune_library.Map_Resoures;
using dune_library.Player_Resources;
using dune_library.Spice;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static dune_library.Utils.Exceptions;

namespace dune_library.Utils {
  public interface I_Faction_Dependent_Initializator_And_Getters {
    public bool Init_Faction_Dependent_Objects();

    public Player_Markers Player_Markers { get; }

    public Alliances Alliances { get; }

    public Forces Reserves { get; }

    public Tleilaxu_Tanks Tleilaxu_Tanks { get; }

    public Knowledge_Manager Knowledge_Manager { get; }

    public Final_Factions_Distribution Final_Factions_Distribution { get; }
  }
}
