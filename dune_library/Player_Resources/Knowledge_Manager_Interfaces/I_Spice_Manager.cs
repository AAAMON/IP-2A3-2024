using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources.Knowledge_Manager_Interfaces {
  public interface I_Spice_Manager {
    public class Faction_Does_Not_Have_Enough_Spice : ArgumentException {
      public Faction Faction { get; }

      public uint Spice_Owned { get; }

      public uint Spice_Requested { get; }

      public Faction_Does_Not_Have_Enough_Spice(Faction faction, uint owned, uint requested) :
        base("Faction (" + faction + ") does not have enough spice to pay the requested amount (spice owned: " + owned + ", spice requested: " + requested + ")") {
        Faction = faction;
        Spice_Owned = owned;
        Spice_Requested = requested;
      }
    }
    public void Transfer_Spice_To(Faction sender, Option<Faction> reciever, uint to_transfer);

    public void Add_Spice_To(Faction reciever, uint to_add);

    public bool Remove_Spice_From(Faction source, uint to_remove);
  }
}
