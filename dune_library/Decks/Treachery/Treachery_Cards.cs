using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Decks.Treachery {
  public static class Treachery_Cards {
    private static I_Occurence_Dict_Read_Only<Treachery_Card> Default_Deck_Composition { get; } = new Occurence_Dict<Treachery_Card> {
    //projectile weapons
    [Treachery_Card.Crysknife] = 1,
    [Treachery_Card.Maula_Pistol] = 1,
    [Treachery_Card.Slip_Tip] = 1,
    [Treachery_Card.Stunner] = 1,
    
    //poison weapons
    [Treachery_Card.Chaumas] = 1,
    [Treachery_Card.Chaumurky] = 1,
    [Treachery_Card.Ellaca_Drug] = 1,
    [Treachery_Card.Gom_Jabbar] = 1,

    [Treachery_Card.Lasgun] = 1,

    [Treachery_Card.Shield] = 4,
    [Treachery_Card.Snooper] = 4,

    [Treachery_Card.Cheap_Hero] = 3,
    [Treachery_Card.Family_Atomics] = 1,
    [Treachery_Card.Hajr] = 1,
    [Treachery_Card.Karama] = 2,
    [Treachery_Card.Tleilaxu_Ghola] = 1,
    [Treachery_Card.Truthtrance] = 2,
    [Treachery_Card.Weather_Control] = 1,

    //worthless cards
    [Treachery_Card.Baliset] = 1,
    [Treachery_Card.Jubba_Cloak] = 1,
    [Treachery_Card.Kulon] = 1,
    [Treachery_Card.La_La_La] = 1,
    [Treachery_Card.Trip_To_Gamont] = 1,
    };
  }
}
