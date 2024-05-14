using dune_library.Decks.Treachery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static dune_library.Decks.Treachery.Treachery_Cards;

namespace dune_library.Player_Resources {
  public interface I_Faction_Knowledge : I_Faction_Knowledge_Read_Only {
    public class Traitors_Have_Already_Been_Initialized : InvalidOperationException {
      public Traitors_Have_Already_Been_Initialized() :
        base("The traitors for this faction have already been initialized") { }
    }

    public void Add_Spice(uint to_add);

    public bool Remove_Spice(uint to_remove);

    public void Add_Treachery_Card(Treachery_Card to_add);

    public bool Remove_Treachery_Card(Treachery_Card to_remove);

    public void Init_Traitors(IReadOnlyList<General> traitors, IReadOnlyList<General> discarded_traitors);

    public void Add_To_Number_Of_Treachery_Cards_Of(Faction faction);

    public bool Remove_From_Number_Of_Treachery_Cards_Of(Faction faction);
  }
}
