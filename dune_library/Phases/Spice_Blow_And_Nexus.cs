using dune_library.Decks.Spice;
using dune_library.Map_Resources;
using dune_library.Player_Resources;
using dune_library.Utils;
using System;
using System.Security.Policy;
using static System.Collections.Specialized.BitVector32;

namespace dune_library.Phases {
  internal class Spice_Blow_And_Nexus : Phase
  {
      private Game game;

        public override string name => "Spice Blow and Nexus";

        internal Spice_Deck Spice_Deck { get; }

        public override string moment { get; protected set; } = "";

        private uint turn;

        private I_Perspective_Generator Perspective_Generator { get; }

        private IReadOnlySet<Player> Players { get; }

        public Spice_Blow_And_Nexus(Game game)
      {
          this.game = game;
          Spice_Deck = game.Spice_Deck;
          turn = game.Round;
          Players = game.Players;
          Perspective_Generator = game;
      }

      public override void Play_Out()
      {
          moment = "before choosing a spice card";
          Spice_Card topCard = Spice_Deck.Take_Next_Card();
            moment = "spice card was chosen";
          while (topCard is Shai_Hulud_Card && turn == 1) { 
              topCard = Spice_Deck.Take_Next_Card();
          }

          if (topCard is Shai_Hulud_Card)
          {
              HandleShaiHuludCard(topCard);
              while (topCard is Shai_Hulud_Card)
              {
                  topCard = Spice_Deck.Take_Next_Card();
              }
              HandleTerritoryCard((Territory_Card)topCard);
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
          //se asteapta lista cu jucatori
          
      }

      private void HandleTerritoryCard(Territory_Card card)
      {
          if(game.Map.To_Section_With_Spice(card.Section_Position_In_List).Origin_Sector == game.Map.Storm_Sector) {
              Console.WriteLine("The Spice Blow icon is currently in the storm. No spice is placed this turn.");
              return;
          }
          game.Map.To_Section_With_Spice(card.Section_Position_In_List).Add_Spice();

          Console.WriteLine($"Spice Blow in sector {game.Map.To_Section_With_Spice(card.Section_Position_In_List).Id}. {game.Map.To_Section_With_Spice(card.Section_Position_In_List).Spice_Capacity} spice added to the territory.");
          game.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(game.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{game.Factions_Distribution.Player_Of(faction).Id}.json"));
        }
  }
}
