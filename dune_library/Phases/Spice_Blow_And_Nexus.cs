using dune_library.Decks.Spice;
using dune_library.Map_Resources;
using dune_library.Player_Resources;
using dune_library.Utils;
using System;
using System.Security.Policy;

namespace dune_library.Phases {
  /*internal class SpiceBlowPhase : Phase
  {
      private Game game;

      public SpiceBlowPhase(Game game)
      {
          this.game = game;
      }

      public override void Play_Out()
      {
          Spice_Card topCard = game.Spice_Deck.DrawTopCard();

          if (topCard is Shai_Hulud_Card)
          {
              HandleShaiHuludCard(topCard);
          }
          else if (topCard is Territory_Card)
          {
              HandleTerritoryCard((Territory_Card)topCard);
          }
          else
          {
              Console.WriteLine("Unknown card type drawn from Spice Deck.");
          }
      }

      private void HandleShaiHuludCard(Spice_Card card)
      {
          Console.WriteLine("Shai-Hulud card drawn. Initiating Nexus Phase...");
          // logic for Nexus Phase
          // start nexus_asking_thread
      }

      private void HandleTerritoryCard(Territory_Card card)
      {
          if (game.Map.Storm.IsInStorm(card.Sector))
          {
              Console.WriteLine("The Spice Blow icon is currently in the storm. No spice is placed this turn.");
              return;
          }

          Sector sector = game.Map.GetSector(card.Sector);
          sector.AddSpice(card.Amount);

          game.Spice_Deck.Discard(card);

          Console.WriteLine($"Spice Blow in sector {card.Sector}. {card.Amount} spice added to the territory.");
      }

    public void Handling_Spice_Card(Map_Resources.Map map) {
      var card = Take_Next_Card();
      ((Action)(card switch {
        Territory_Card territory_card => () => {
          var section_with_spice = map.To_Section_With_Spice(territory_card.Section_Position_In_List);
          section_with_spice.Add_Spice();
        }
        ,
        Shai_Hulud_Card shai_hulud_card => () => {
          var last_section_with_spice = map.To_Section_With_Spice((Top_OF_Discard_Pile.ValueUnsafe() as Territory_Card)!.Section_Position_In_List);
        }
        ,
        _ => () => throw new Exception(),
      })).Invoke();
    }
  }*/
}
