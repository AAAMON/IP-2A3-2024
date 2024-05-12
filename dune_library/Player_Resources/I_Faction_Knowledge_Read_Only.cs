using dune_library.Decks.Treachery;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public interface I_Faction_Knowledge_Read_Only {
    public uint Spice { get; }

    public I_Occurence_Dict_Read_Only<Treachery_Card> Treachery_Cards { get; }

    public IReadOnlyList<General> Traitors { get; }

    public IReadOnlyList<General> Discarded_Traitors { get; }

    IReadOnlyDictionary<Faction, uint> Number_Of_Treachery_Cards_Of_Other_Factions { get; }

    public uint Number_Of_Treachery_Cards_Of(Faction faction);

    public bool Number_Of_Treachery_Cards_Is_Not_Zero(Faction faction);
  }
}
