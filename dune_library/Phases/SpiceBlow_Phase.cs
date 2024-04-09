using dune_library.Map_Resources;
using dune_library.Player_Resources;
using dune_library.Utils;
using System;
using System.Security.Policy;

namespace dune_library.Phases
{
    internal class SpiceBlowPhase : Phase
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
    }
}
